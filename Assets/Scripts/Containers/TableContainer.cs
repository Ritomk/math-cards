using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class TableContainer : CardContainerBase
{
    [SerializeField] private float initialSpacing = -5.5f;
    [SerializeField] private float maxWidth = 18f;
    [SerializeField] private float yIncrement = 0.01f;
    [SerializeField] private float baseXOffset = -2f;
    [SerializeField] private float baseYOffset = 0.5f;

    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
        soContainerEvents.OnEvaluateExpression += HandleEvaluateRpn;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        soContainerEvents.OnChangeCardsState -= HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement -= ValidateCardPlacement;
        soContainerEvents.OnEvaluateExpression -= HandleEvaluateRpn;
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
                CoroutineHelper.Start(BurnCard(CardsDictionary.Last().Value.CardId));
                return;
            }
        }
    }

    private void UpdateCardPositions()
    {
        if(currentCardCount == 0) return;
        
        float spacing = initialSpacing;
        float totalWidth = Math.Abs(spacing) * (currentCardCount - 1);

        if (totalWidth > maxWidth)
        {
            spacing = -maxWidth / (currentCardCount - 1);
        }

        var index = 0;
        foreach (var card in CardsDictionary.Values)
        {
            float xPosition = baseXOffset + (spacing * index);
            float yPosition = baseYOffset + (yIncrement * index);
            card.transform.localPosition = new Vector3(xPosition, yPosition, 0);
            card.transform.rotation = transform.rotation;
            ++index;
        }
    }

    private void HandleEvaluateRpn()
    {
        var expression = CardsDictionary
            .Select(kvp => kvp.Value.Token)
            .ToList();

        RpnExpressionHelper.EvaluateRpnExpression(expression, out float result);
        
        soContainerEvents.RaiseSendExpressionResult(result, SelfContainerKey);
    }

    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in CardsDictionary.Values)
        {
            card.State = newState;
        }
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
