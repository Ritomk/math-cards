using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableContainer : CardContainerBase
{
    [SerializeField] private SoContainerEvents soContainerEvents;

    private void OnEnable()
    {
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
    }

    private void OnDisable()
    {
        soContainerEvents.OnChangeCardsState -= HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement -= ValidateCardPlacement;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(EvaluateExpression());
        }
    }

    public override bool AddCard(Card card)
    {
        bool result = base.AddCard(card);
        if (result)
        {
            UpdateCardPositions();
        }
        
        return result;
    }

    protected override void ValidateCardPlacement()
    {
        var cos = 0;
        foreach (var card in cardsDictionary.Values)
        {
            switch (card.TokenType)
            {
                case CardData.TokenType.SingleDigit:
                case CardData.TokenType.DoubleDigit:
                    ++cos;
                    break;
                case CardData.TokenType.Symbol:
                    --cos;
                    break;
            }

            if (cos <= 0)
            {
                BurnCard(cardsDictionary.Last().Value.CardId);
                return;
            }
        }
    }

    private void UpdateCardPositions()
    {
        float spacing = -5.5f;
        int index = 1;
        foreach (var card in cardsDictionary.Values)
        {
            card.transform.localPosition = new Vector3(spacing * index, 0.2f, 0);
            card.transform.rotation = Quaternion.Euler(0, 0, 0);
            ++index;
        }
    }

    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in cardsDictionary.Values)
        {
            card.State = newState;
        }
    }

    public float EvaluateExpression()
    {
        List <string> tokens = new List<string> ();
        foreach (var card in cardsDictionary.Values)
        {
            tokens.Add(card.Token);
        }

        return EvaluateRpn(tokens);
    }

    private float EvaluateRpn(List<string> tokens)
    {
        Stack<float> stack = new Stack<float>();
        foreach (var token in tokens)
        {
            if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                float b = stack.Pop();
                float a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    default: throw new InvalidOperationException($"Invalid operator {token}");
                }
            }
        }
        return stack.Pop();
    }
}
