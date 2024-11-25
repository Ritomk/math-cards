using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RPNExampleUsage : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    private Dictionary<int,Card> cards = new Dictionary<int, Card>();
    private static int cardId = -1;
    private RpnExpressionGenerator generator;
    [SerializeField] private TMP_Text textMeshPro;

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
        
        generator = gameObject.AddComponent<RpnExpressionGenerator>();

    }
    //pay to friend 26zl for pizza
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Temp();
        }
    }

    private void PrintDictionary()
    {
        var debugLog = "";
        foreach (var card in cards.Values)
        {
            debugLog += $"{card.Token} ";
        }
        Debug.Log($"Expression string: {debugLog}");
    }

    private void PrintExpression(List<int> tokens)
    {
        var debugLog = "";
        foreach (var token in tokens)
        {
            debugLog += $"{token} ";
        }
        Debug.Log($"Expression int: {debugLog}");
    }
    
    private Dictionary<int,Card> RemoveCardsUsedInExpression(List<int> expression)
    {
        Dictionary<int,Card> usedCards = new Dictionary<int, Card>(cards);
        foreach (var token in expression)
        {
            var strToken = "";
            if (token > 100)
            {
                TokenMapping.IntToStringMap.TryGetValue(token, out strToken);
            }
            else
            {
                strToken = token.ToString();
            }
            // Find the first matching card
            foreach (var entry in usedCards)
            {
                // if (entry.Value.Token == strToken)
                // {
                //     usedCards.Remove(entry.Key);
                //     //Debug.Log($"Removed card: {entry.Value.Token}");
                //     break;
                // }
            }
        }
        return usedCards;
    }

    private async void Temp1()
    {
        int maxLength = 7;
        textMeshPro.text = "Liczy";
        float startTime = Time.realtimeSinceStartup;
        var expression = await generator.StartComputation(new List<int>(), cards, maxLength);
        float endTime = Time.realtimeSinceStartup;
        Debug.Log($"Calculation finished");
        Debug.Log($"Elapsed time: {endTime - startTime}");
        Debug.Log($"Expression: {RpnExpressionHelper.ExpressionToString(expression)}");
        textMeshPro.text = "Skonczylo";
    }


    private async void Temp()
    {
        PrintDictionary();

        int maxLength = 7;
        textMeshPro.text = "Liczy1";
        var expression = await generator.StartComputation(new List<int>(), cards, maxLength);
        
        var usedCards = RemoveCardsUsedInExpression(expression);
        
        maxLength = 9;
        expression = await generator.StartComputation(expression, usedCards, maxLength);
        RpnExpressionGenerator.EvaluateRpnExpression(expression, out float result);
        Debug.Log("Expression 1 ---------------");
        PrintExpression(expression);
        Debug.Log(RpnExpressionHelper.ExpressionToString(expression));
        Debug.Log(result);
        
        textMeshPro.text = "Skonczylo1";

        maxLength = 5;
        textMeshPro.text = "Liczy2";
        expression = await generator.StartComputation(new List<int>(), cards, maxLength);
        
        usedCards = RemoveCardsUsedInExpression(expression);
        
        maxLength = 9;
        expression = await generator.StartComputation(expression, usedCards, maxLength);
        RpnExpressionGenerator.EvaluateRpnExpression(expression, out result);
        Debug.Log("Expression 2 ---------------");
        PrintExpression(expression);
        Debug.Log(RpnExpressionHelper.ExpressionToString(expression));
        Debug.Log(result);
        
        textMeshPro.text = "Skonczylo2";
        
        maxLength = 3;
        textMeshPro.text = "Liczy3";
        expression = await generator.StartComputation(new List<int>(), cards, maxLength);
        
        usedCards = RemoveCardsUsedInExpression(expression);
        
        maxLength = 9;
        expression = await generator.StartComputation(expression, usedCards, maxLength);
        RpnExpressionGenerator.EvaluateRpnExpression(expression, out result);
        Debug.Log("Expression 3 ---------------");
        PrintExpression(expression);
        Debug.Log(RpnExpressionHelper.ExpressionToString(expression));
        Debug.Log(result);
        
        textMeshPro.text = "Skonczylo3";
    }

    private void InstantiateCard(string token)
    {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        var card = cardObject.GetComponent<Card>();
        cardObject.name = $"Card {token}";
        // card.Initialize(token, false, CardData.CardState.NonPickable);
        cards.Add(cardId, card);
        ++cardId;
    }
}