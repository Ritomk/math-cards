using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{
    [Category("Card Tasks")]
    public class HasWon : ActionTask
    {
        [BlackboardOnly] public BBParameter<SoContainerEvents> soContainerEvents = new BBParameter<SoContainerEvents>()
            { name = "Container Events" };

        [BlackboardOnly] public BBParameter<bool> hasEnemyWon = new BBParameter<bool>()
            { name = "Has Enemy Won" };
        
        private Dictionary<ContainerKey, float> _rpnResults = new Dictionary<ContainerKey, float>();
        private bool _winnerDecided = false;
        
        protected override string OnInit() {
            return null;
        }

        protected override void OnExecute()
        {
            CoroutineHelper.Start(ExecuteCheck());
            
            EndAction(true);
        }

        private IEnumerator ExecuteCheck()
        {
            _winnerDecided = false;
            soContainerEvents.value.OnSendExpressionResult += HandleReceiveRpnFromTables;
            
            soContainerEvents.value.RaiseEvaluateExpression();
            
            yield return new WaitUntil(() => _winnerDecided);
            
            soContainerEvents.value.OnSendExpressionResult -= HandleReceiveRpnFromTables;
        }
        
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

            if (playerVsEnemy > enemyVsPlayer)
            {
                hasEnemyWon.value = false;
                Debug.Log("Player Wins the Attack Comparison!");
            }
            else if (enemyVsPlayer > playerVsEnemy)
            {
                hasEnemyWon.value = true;
                Debug.Log("Enemy Wins the Attack Comparison!");
            }
            else
            {
                hasEnemyWon.value = false;
                Debug.Log("Attack Comparison is a tie!");
            }
            
            _rpnResults.Clear();
            _winnerDecided = true;
        }
    }
}
