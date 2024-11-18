using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RPNExampleUsage : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    private Dictionary<int,Card> cards = new Dictionary<int, Card>();
    private static int cardId = -1;

    private async void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            InstantiateCard(i.ToString());
        }
    
        for (int i = 0; i < 7; i++)
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
        int maxLength = 7;
        
        float startTime = Time.realtimeSinceStartup;
        var expression = await generator.StartComputation(cards, maxLength);
        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"Calculation finished");
        Debug.Log($"Elapsed time: {endTime - startTime}");
        Debug.Log($"Expression: {RpnExpressionGenerator.ExpressionToString(expression)}");
    }

    private void Awake()
    {
        Test();
    }

    private async void Test() => await Task.Delay(1000);

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