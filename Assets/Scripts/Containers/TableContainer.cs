using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableContainer : CardContainerBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        soContainerEvents.OnChangeCardsState -= HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement -= ValidateCardPlacement;
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
        foreach (var card in CardsDictionary.Values)
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
                BurnCard(CardsDictionary.Last().Value.CardId);
                return;
            }
        }
    }

    private void UpdateCardPositions()
    {
        float spacing = -5.5f;
        int index = 1;
        foreach (var card in CardsDictionary.Values)
        {
            card.transform.localPosition = new Vector3(spacing * index, 0.2f, 0);
            card.transform.rotation = Quaternion.Euler(0, 0, 0);
            ++index;
        }
    }

    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in CardsDictionary.Values)
        {
            card.State = newState;
        }
    }

    public float EvaluateExpression()
    {
        List <string> tokens = new List<string> ();
        foreach (var card in CardsDictionary.Values)
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

    protected override void HandleCardData(EnemyKnowledgeData data)
    {
        if (SelfContainerKey.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.AttackTable)))
        {
            data.selfAttackTableList = CardsDictionary.Select(x => x.Value.Token).ToList();
        }
        else if (SelfContainerKey.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.DefenceTable)))
        {
            data.selfDefenceTableList = CardsDictionary.Select(x => x.Value.Token).ToList();
        }
        else if (SelfContainerKey.Equals(new ContainerKey(OwnerType.Player, CardContainerType.AttackTable)))
        {
            data.playerAttackTableList = CardsDictionary.Select(x => x.Value.Token).ToList();
        }
        else if (SelfContainerKey.Equals(new ContainerKey(OwnerType.Player, CardContainerType.DefenceTable)))
        {
            data.playerDefenceTableList = CardsDictionary.Select(x => x.Value.Token).ToList();
        }
    }
}
