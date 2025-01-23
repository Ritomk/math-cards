using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions {

    [Category("Card Tasks")]
    public class IsWinning : ActionTask {

        [BlackboardOnly] public BBParameter<SoGameStateEvents> gameStateEvents = new BBParameter<SoGameStateEvents>()
            { name = "Game State Event" };
        
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };
        
        [BlackboardOnly] public BBParameter<float> calculatedChance = new BBParameter<float>()
            { name = "Calculated Chance" };
        [BlackboardOnly] public BBParameter<float> invertedCalculatedChance = new BBParameter<float>()
            { name = "Inverted Calculated Chance" };
		
        protected override string OnInit() {
            return null;
        }

        protected override void OnExecute()
        {
            CalculateWinning();
            invertedCalculatedChance.value = 100 - calculatedChance.value;
            EndAction(true);
        }

        private void CalculateWinning()
        {
            // Calculate values based on enemy and player stats
            RpnExpressionHelper.EvaluateRpnExpressionLeader(knowledgeData.value.selfAttackTableList,
                out var selfAttackResult);
            RpnExpressionHelper.EvaluateRpnExpressionLeader(knowledgeData.value.selfDefenceTableList,
                out var selfDefenseResult);
            RpnExpressionHelper.EvaluateRpnExpressionLeader(knowledgeData.value.playerAttackTableList,
                out var playerAttackResult);
            RpnExpressionHelper.EvaluateRpnExpressionLeader(knowledgeData.value.playerDefenceTableList,
                out var playerDefenseResult);

            float difference = (playerAttackResult - selfDefenseResult) - (selfAttackResult - playerDefenseResult);

            if (gameStateEvents.value.playerHasEndedRound && difference > 0)
            {
                calculatedChance.value = 100f;
                return;
            }
            
            switch (difference)
            {
                case > 150:
                    calculatedChance.value *= 4f;
                    break;
                case > 50 and <= 150:
                    calculatedChance.value *= 2f;
                    break;
                case >= 0 and <= 50:
                    // No change
                    break;
                case >= -50 and < 0:
                    calculatedChance.value *= 0.5f;
                    break;
                case >= -150 and < -50:
                    calculatedChance.value *= 0.75f;
                    break;
                case < -150:
                    calculatedChance.value *= 4f;
                    break;
            }
        }
    }
}