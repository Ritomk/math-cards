using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{
    [Category("Card Conditions")]
    public class CheckMessWithPlayer : ConditionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> enemyKnowledgeData =
            new BBParameter<EnemyKnowledgeData>() { name = "Enemy Knowledge Data" };
		
        [BlackboardOnly] public BBParameter<Dictionary<int, Card>> availableCardsPool =
            new BBParameter<Dictionary<int, Card>>() { name = "Available Cards Pool" };
        
        [BlackboardOnly] public BBParameter<Card> chosenCardToPlace = new BBParameter<Card>()
            { name = "Chosen Card To Place" };
        
        [BlackboardOnly] public BBParameter<bool> placeOnAttackTable = new BBParameter<bool>()
            { name = "Should Place On Attack Table" };
        
        [BlackboardOnly] [SliderField(0, 100)] public BBParameter<float> chanceToMess = new BBParameter<float>()
            { name = "Chance To Mess", value = 5f };

        private const int MAX_EXPRESSION_LENGTH = 9;
        private const int MAX_OPERANDS = 5;
        private const float BASE_CHANCE_RESET = 20f;
        
        protected override string info
        {
            get { return $"Chance To Mess = {chanceToMess.value:F2}%"; }
        }

        protected override bool OnCheck()
        {
            float cheatChance = Mathf.Clamp(chanceToMess.value, 0f, 100f);
            
            float roll = Random.Range(0f, 100f);
            bool willCheat = (roll < cheatChance);

            if (!willCheat)
            {
                float r = Random.value;
                float factor = 1f + (r * r);
                float newChance = cheatChance * factor;

                newChance = Mathf.Min(newChance, 100f);
                chanceToMess.value = newChance;
                
                return false;
            }
            chanceToMess.value = BASE_CHANCE_RESET;
            
            var playerAttackTable = enemyKnowledgeData.value.playerAttackTableList;
            var playerDefenseTable = enemyKnowledgeData.value.playerDefenceTableList;

            RpnExpressionHelper.EvaluateRpnExpressionAll(playerAttackTable, out List<float> resultsAttack);
            RpnExpressionHelper.EvaluateRpnExpressionAll(playerDefenseTable, out List<float> resultsDefense);

            float sumAttack  = resultsAttack.Sum();
            float sumDefense = resultsDefense.Sum();
            
            bool hamperAttack = (sumAttack >= sumDefense);
            
            var primaryTable = hamperAttack ? playerAttackTable : playerDefenseTable;
            var secondaryTable = hamperAttack ? playerDefenseTable : playerAttackTable;

            var cardToUse = FindMessingCardForTable(primaryTable, availableCardsPool.value);

            if (cardToUse != null)
            {
                chosenCardToPlace.value = cardToUse;
                placeOnAttackTable.value = hamperAttack;
                return true;
            }
            else
            {
                bool hamperAttackSecondary = !hamperAttack;
                cardToUse = FindMessingCardForTable(secondaryTable, availableCardsPool.value);

                if (cardToUse != null)
                {
                    chosenCardToPlace.value = cardToUse;
                    placeOnAttackTable.value = hamperAttackSecondary;
                    return true;
                }

                return false;
            }
            
            return true;
        }

        private Card FindMessingCardForTable(List<float> table, Dictionary<int, Card> pool)
        {
            if(table.Count >= MAX_EXPRESSION_LENGTH)
                return null;

            int operatorCount = table.Count(RpnExpressionHelper.IsOperator);
            int operandCount = table.Count - operatorCount;
            
            int stackHeight = operandCount - operatorCount;
            
            var messingCandidates = new List<Card>();

            foreach (var card in pool.Values)
            {
                if (card.TokenType == CardData.TokenType.Symbol)
                {
                    if (stackHeight >= 2 && IsMessingOperator(card.Token))
                    {
                        messingCandidates.Add(card);
                    }
                }
                else
                {
                    if (operandCount < MAX_OPERANDS && IsSmallPositiveOperand(card.Token))
                    {
                        messingCandidates.Add(card);
                    }
                }
            }
            
            if(messingCandidates.Count == 0)
                return null;
            Debug.Log($"Dupa: {string.Join(" ", messingCandidates)}");
            var chosen = messingCandidates[Random.Range(0, messingCandidates.Count)];
            pool.Remove(chosen.CardId);

            return chosen;
        }
        
        private bool IsMessingOperator(float token)
        {
            if (!RpnExpressionHelper.IsOperator(token)) return false;
            
            return Mathf.Approximately(token, 0.06f) || Mathf.Approximately(token, 0.08f);
        }
        
        private bool IsSmallPositiveOperand(float token)
        {
            return (token > 0 && token < 3f);
        }
    }
}