using System.Collections.Generic;
using UnityEngine;

public class RPNExampleUsage : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    private Dictionary<int,Card> cards = new Dictionary<int, Card>();
    private static int cardId = -1;
    private void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            InstantiateCard(i.ToString());
        }

        for (int i = 0; i < 4; i++)
        {

            var randomIndex = UnityEngine.Random.Range(0, 4);
            var sign = randomIndex switch
            {
                0 => "+",
                1 => "-",
                2 => "\u00d7",
                3 => "\u00f7",
                _ => "0"
            };
            InstantiateCard(sign);
        }
        
        RpnExpressionGenerator generator = gameObject.AddComponent<RpnExpressionGenerator>();
        generator.OnComputationComplete += OnComputationComplete;

        int maxLength = 5;
        generator.StartComputation(cards, maxLength);
    }
    
    private void OnComputationComplete(List<int> maxExpression, float maxResult)
    {
        // This runs on the main thread
        Debug.Log($"Expression with highest value: {ExpressionToString(maxExpression)} = {maxResult}");
    }
    
    private static string ExpressionToString(List<int> expression)
    {
        List<string> tokensAsString = new List<string>();
        foreach (int token in expression)
        {
            if (TokenMapping.IntToStringMap.TryGetValue(token, out string opStr))
            {
                tokensAsString.Add(opStr);
            }
            else
            {
                tokensAsString.Add(token.ToString());
            }
        }
        return string.Join(" ", tokensAsString);
    }

    private void InstantiateCard(string token)
    {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        var card = cardObject.GetComponent<Card>();
        cardObject.name = $"Card {token}";
        card.Initialize(token, false, CardData.CardState.NonPickable);
        cards.Add(cardId, card);
        ++cardId;
    }
}