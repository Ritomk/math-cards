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
            _soContainerEvents.OnSendExpressionResult += HandleReceiveRpnFromTables;
            
            yield return CoroutineHelper.StartAndWait(ReturnCardsToDeck());
            
            _soContainerEvents.RaiseEvaluateExpression();
            yield return new WaitUntil(() => _winnerDecided);
            
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.BeginRound);
        }

        public override IEnumerator Exit()
        {
            _soContainerEvents.OnSendExpressionResult -= HandleReceiveRpnFromTables;
            
            _winnerDecided = false;
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

            if (_rpnResults.ContainsKey(playerAttackKey) &&
                _rpnResults.ContainsKey(playerDefenceKey) &&
                _rpnResults.ContainsKey(enemyAttackKey) &&
                _rpnResults.ContainsKey(enemyDefenceKey))
            {
                float playerAttackValue  = _rpnResults[playerAttackKey];
                float playerDefenceValue = _rpnResults[playerDefenceKey];
                float enemyAttackValue   = _rpnResults[enemyAttackKey];
                float enemyDefenceValue  = _rpnResults[enemyDefenceKey];
                
                DecideWinner(playerAttackValue, playerDefenceValue, enemyAttackValue, enemyDefenceValue);
            }
        }

        private void DecideWinner(float playerAttack, float enemyDefence, float enemyAttack, float playerDefence)
        {
            float playerVsEnemy = playerAttack - enemyDefence;
            float enemyVsPlayer = enemyAttack - playerDefence;

            Debug.Log($"Player Attack - Enemy Defence = {playerVsEnemy};  Enemy Attack - Player Defence = {enemyVsPlayer}");

            var playerWon = false;
            if (playerVsEnemy > enemyVsPlayer)
            {
                // Player has the stronger strike
                playerWon = true;
                Debug.Log("Player Wins the Attack Comparison!");
            }
            else if (enemyVsPlayer > playerVsEnemy)
            {
                // Enemy has the stronger strike
                playerWon = false;
                Debug.Log("Enemy Wins the Attack Comparison!");
            }
            else
            {
                // It's a tie
                playerWon = false;
                Debug.Log("Attack Comparison is a tie!");
            }
            
            _soGameStateEvents.RaisePlayerWonRound(playerWon);
            _rpnResults.Clear();
            _winnerDecided = true;
        }
    }   
}