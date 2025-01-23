using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class EndRoundState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.EndRound;
        
        private SoGameStateEvents _soGameStateEvents;
        private SoCardEvents _soCardEvents;
        private SoCardEvents _opponentSoCardEvents;
        private SoContainerEvents _soContainerEvents;

        private float _timeBetweenDraws = 0.1f;
        private bool _winnerDecided = false;
        
        private Dictionary<ContainerKey, float> _rpnResults = new Dictionary<ContainerKey, float>();
        
        public EndRoundState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents, SoCardEvents soCardEvents, SoCardEvents opponentSoCardEvents, SoContainerEvents soContainerEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
            _opponentSoCardEvents = opponentSoCardEvents;
            _soContainerEvents = soContainerEvents;
        }

        public override IEnumerator Enter()
        {
            _soGameStateEvents.RaisePlayerStateChange(PlayerStateEnum.BeginRound);
            
            _soContainerEvents.OnSendExpressionResult += HandleReceiveRpnFromTables;
            
            _soContainerEvents.RaiseEvaluateExpression();
            _soContainerEvents.RaiseBurnMergedCards();
            yield return new WaitUntil(() => _winnerDecided);
            
            _soGameStateEvents.RaisePlayerStateChange(PlayerStateEnum.EndRound);
        }

        public override IEnumerator Exit()
        {
            _soContainerEvents.OnSendExpressionResult -= HandleReceiveRpnFromTables;
            _winnerDecided = false;
            
            _soContainerEvents.RaiseClearTables();
            
            yield return CoroutineHelper.StartAndWait(ReturnCardsToDeck());
            
            _soContainerEvents.RaiseReshuffleCards();
            
            yield return null;
        }

        private IEnumerator ReturnCardsToDeck()
        {
            var shouldContinuePlayer = true;
            var shouldContinueOpponent = true;
            
            while (true)
            {
                shouldContinuePlayer = TryDrawCard(_soCardEvents, shouldContinuePlayer);
                shouldContinueOpponent = TryDrawCard(_opponentSoCardEvents, shouldContinueOpponent);
                
                if(!shouldContinuePlayer && !shouldContinueOpponent)
                    yield break;
                yield return new WaitForSecondsPauseable(_timeBetweenDraws);
            }
        }

        private bool TryDrawCard(SoCardEvents soCardEvents, bool canStillDraw) =>
            canStillDraw && soCardEvents.RaiseCardDraw(false);

        private void HandleReceiveRpnFromTables(float rpnResult, ContainerKey containerKey)
        {
            _rpnResults[containerKey] = rpnResult;
            TryResolveAllResults();
        }

        private void TryResolveAllResults()
        {
            if(_rpnResults.Count < 4) return;
            
            var playerAttackKey   = new ContainerKey(OwnerType.Player, CardContainerType.AttackTable);
            var playerDefenceKey  = new ContainerKey(OwnerType.Player, CardContainerType.DefenceTable);
            var enemyAttackKey    = new ContainerKey(OwnerType.Enemy,  CardContainerType.AttackTable);
            var enemyDefenceKey   = new ContainerKey(OwnerType.Enemy,  CardContainerType.DefenceTable);

            if (_rpnResults.TryGetValue(playerAttackKey, out var playerAttackValue) &&
                _rpnResults.TryGetValue(playerDefenceKey, out var playerDefenceValue) &&
                _rpnResults.TryGetValue(enemyAttackKey, out var enemyAttackValue) &&
                _rpnResults.TryGetValue(enemyDefenceKey, out var enemyDefenceValue))
            {
                DecideWinner(playerAttackValue, playerDefenceValue, enemyAttackValue, enemyDefenceValue);
            }
        }
        
        private void DecideWinner(float playerAttack, float playerDefence, float enemyAttack, float enemyDefence)
        {
            float playerVsEnemy = Mathf.Max(playerAttack - enemyDefence, 0);
            float enemyVsPlayer = Mathf.Max(enemyAttack - playerDefence, 0);

            Debug.Log($"Player Attack - Enemy Defence = {playerVsEnemy};  Enemy Attack - Player Defence = {enemyVsPlayer}");

            var playerWon = false;
            if (playerVsEnemy > enemyVsPlayer)
            {
                playerWon = true;
                Debug.Log("Player Wins the Attack Comparison!");
            }
            else if (enemyVsPlayer > playerVsEnemy)
            {
                playerWon = false;
                Debug.Log("Enemy Wins the Attack Comparison!");
            }
            else
            {
                playerWon = false;
                Debug.Log("Attack Comparison is a tie!");
            }
            
            _soGameStateEvents.RaisePlayerWonRound(playerWon);
            _rpnResults.Clear();
            _winnerDecided = true;
        }
    }   
}