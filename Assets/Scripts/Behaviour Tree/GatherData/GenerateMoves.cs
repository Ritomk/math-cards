using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        [BlackboardOnly] public BBParameter<List<float>> tableList =
            new BBParameter<List<float>>() { name = "Self Attack Table List" };

        [BlackboardOnly] public BBParameter<Dictionary<int, Card>> availableCardsPool =
            new BBParameter<Dictionary<int, Card>>() { name = "Available Cards Pool" };

        public int maxExpressionLength = 5;
        public int partitionSize = 3;
        
        
        protected override void OnExecute()
        {
            ReturnCardsToPool();
            
            var initialCards = new List<float>(tableList.value);
            partitionSize = CalculatePartitionSize(initialCards);
            
            GenerateExpressions(initialCards);
        }

        private int CalculatePartitionSize(List<float> initialCards)
        {
            var chosenPartitionSize = Random.Range(0, 2) == 0 ? 3 : 5;

            return initialCards.Count >= chosenPartitionSize ? 0 : chosenPartitionSize;
        }

        private async void GenerateExpressions(List<float> initialCards)
        {
            float startTime = Time.realtimeSinceStartup;

            var expression1 = await GenerateFirstExpression(initialCards);
            var expression2 = await GenerateSecondExpression(expression1);
            
            tableMoves.value = GetUniqueCardsMatchingTokens(expression2, knowledgeData.value.selfHandCardsDictionary);
            
            float endTime = Time.realtimeSinceStartup;

            Debug.Log("AI: Calculation finished");
            Debug.Log($"AI: Elapsed time: {endTime - startTime}");
            Debug.Log($"AI: Expression: {RpnExpressionHelper.ExpressionToString(expression2)}");

            Debug.Log($"AI: Cards in Pool: {string.Join(" ", availableCardsPool.value.Values.Select(card => card.Token))}");
            Debug.Log($"AI: Cards in Hand: {string.Join(" ", knowledgeData.value.selfHandCardsDictionary.Values.Select(card => card.Token))}");
            EndAction(expression2.Count > 0);
        }

        private async Task<List<float>> GenerateFirstExpression(List<float> initialCards)
        {
            if (partitionSize <= 0)
            {
                // No partition size means skipping expression1
                return new List<float>(initialCards);
            }
            
            var cards = availableCardsPool.value;
            var expression = await generator.value.StartComputation(initialCards, cards, partitionSize);
            
            if (expression == null || expression.Count == 0)
            {
                Debug.Log("AI: Expression1 generation failed or returned nothing. Using initialCards for expression2.");
                return new List<float>(initialCards);
            }

            GetUniqueCardsMatchingTokens(new List<float>(expression), knowledgeData.value.selfHandCardsDictionary);
            return expression;
        }

        private async Task<List<float>> GenerateSecondExpression(List<float> initialCards)
        {
            var cards = availableCardsPool.value;
            var expression = await generator.value.StartComputation(initialCards, cards, maxExpressionLength);
            
            return expression;
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

        private Stack<Card> GetUniqueCardsMatchingTokens(List<float> tokens, Dictionary<int, Card> cards)
        {
            HashSet<int> usedCardIds = new HashSet<int>();
            
            Stack<Card> resultStack = new Stack<Card>();

            tokens.Reverse();
            
            
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