using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Unity.VisualScripting;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Card Tasks")]
    public class GenerateMoves : ActionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };
        
        [BlackboardOnly] public BBParameter<RpnExpressionGenerator> generator =
            new BBParameter<RpnExpressionGenerator>();

        [BlackboardOnly] public BBParameter<Stack<Card>> tableMoves = new BBParameter<Stack<Card>>()
            { name = "Attack Table Moves" };
        
        [BlackboardOnly] public BBParameter<List<string>> tableList =
            new BBParameter<List<string>>() { name = "Self Attack Table List" };

        [BlackboardOnly] public BBParameter<Dictionary<int, Card>> availableCardsPool =
            new BBParameter<Dictionary<int, Card>>() { name = "Available Cards Pool" };

        public int maxExpressionLength = 5;
        private int partitionSize = 3;
        
        
        protected override void OnExecute()
        {
            ReturnCardsToPool();
            
            partitionSize = Random.Range(0, 1) == 0 ? 3 : 5;
            
            GenerateExpression();
        }

        private async void GenerateExpression()
        {
            float startTime = Time.realtimeSinceStartup;
            
            var cards = availableCardsPool.value;
            var initialCards = RpnExpressionHelper.ExpressionToIntList(tableList.value);
            
            var expression = await generator.value.StartComputation( initialCards ,cards, maxExpressionLength);
            var strExpression = RpnExpressionHelper.ExpressionToStringList(expression);

            tableMoves.value = GetUniqueCardsMatchingTokens(strExpression, knowledgeData.value.selfHandCardsDictionary);
            
            float endTime = Time.realtimeSinceStartup;
            
            Debug.Log("AI: Calculation finished");
            Debug.Log($"AI: Elapsed time: {endTime - startTime}");
            Debug.Log($"AI: Expression: {RpnExpressionHelper.ExpressionToString(expression)}");
            
            Debug.Log($"AI: Cards in Pool: {string.Join(" ", availableCardsPool.value.Values.Select(card => card.Token))}");
            Debug.Log($"AI: Cards in Hand: {string.Join(" ", knowledgeData.value.selfHandCardsDictionary.Values.Select(card => card.Token))}");

            EndAction(expression.Count > 0);
        }

        private void ReturnCardsToPool()
        {
            if (tableMoves.value?.Count != 0) return;
            
            availableCardsPool.value.AddRange(tableMoves.value.ToDictionary(card => card.CardId, card => card));
            tableMoves.value.Clear();
            Debug.Log("AI: Cards returned");
            var debugLog = "";

            foreach (var card in availableCardsPool.value.Values)
            {
                debugLog += $"{card.Token} ";
            }
            Debug.Log($"AI: Cards Pool: {debugLog}");
            Debug.Log($"AI: Cards in Hand: {string.Join(" ", knowledgeData.value.selfHandCardsDictionary.Values)}");
        }

        private Stack<Card> GetUniqueCardsMatchingTokens(List<string> tokens, Dictionary<int, Card> cards)
        {
            HashSet<int> usedCardIds = new HashSet<int>();
            
            Stack<Card> resultStack = new Stack<Card>();

            foreach (var token in tokens)
            {
                var card = cards.Values.FirstOrDefault(c => c.Token == token && !usedCardIds.Contains(c.CardId));

                if (card != null)
                {
                    Debug.Log($"AI: Found card {token} at {card.CardId}");
                    availableCardsPool.value.Remove(card.CardId);
                    resultStack.Push(card);
                    
                    usedCardIds.Add(card.CardId);
                }
            }
            return resultStack;
        }
    }
}