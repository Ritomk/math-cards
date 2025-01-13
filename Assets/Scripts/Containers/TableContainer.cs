using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Tasks.Actions;
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
        
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
        soContainerEvents.OnEvaluateExpression += HandleEvaluateRpn;
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnClearTables += HandleClearTables;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        soContainerEvents.OnValidateCardPlacement -= ValidateCardPlacement;
        soContainerEvents.OnEvaluateExpression -= HandleEvaluateRpn;
        soContainerEvents.OnChangeCardsState -= HandleChangeCardsState;
        soContainerEvents.OnClearTables -= HandleClearTables;
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

        RpnExpressionHelper.EvaluateRpnExpressionOrder(CardsDictionary, out float result, out var operationOrder);
        CoroutineHelper.Start(VisualizeRpnEvaluation(CardsDictionary, operationOrder));
        soContainerEvents.RaiseSendExpressionResult(result, SelfContainerKey);
    }

    private IEnumerator VisualizeRpnEvaluation(Dictionary<int, Card> cardsDictionary, List<List<int>> operationOrder)
    {
        Card resultCard = null;
        
        foreach (var operation in operationOrder)
        {
            if (operation.Count < 1) continue;

            int operatorCardId = operation[0];
            int operand1Id = operation[1];
            int operand2Id = operation[2];
            
            if (!cardsDictionary.TryGetValue(operatorCardId, out var operatorCard) ||
                !cardsDictionary.TryGetValue(operand1Id, out var operand1) ||
                !cardsDictionary.TryGetValue(operand2Id, out var operand2))
            {
                Debug.LogError($"Cards for operation not found! Operator: {operatorCardId}, Operand1: {operand1Id}, Operand2: {operand2Id}");
                continue;
            }

            yield return CoroutineHelper.StartAndWait(MoveCardTo(operatorCard.transform.position,
                operand2, 0.5f));
            yield return CoroutineHelper.StartAndWait(MoveCardTo(operatorCard.transform.position,
                operand1, 0.5f));

            float operand1Value = operand1.Token;
            float operand2Value = operand2.Token;
            float resultValue = RpnExpressionHelper.ApplyOperator(operand1Value, operand2Value, operatorCard.Token);

            operatorCard.Token = RpnExpressionHelper.EncodeToken(resultValue);
            
            resultCard = operatorCard;
            
            cardsDictionary.Remove(operand1.CardId);
            cardsDictionary.Remove(operand2.CardId);
            Destroy(operand1.gameObject);
            Destroy(operand2.gameObject);

            UpdateCardPositions();
            yield return new WaitForSecondsPauseable(0.5f);
        }
        
        foreach (var card in cardsDictionary.Values)
        {
            if (resultCard && card.CardId != resultCard.CardId)
            {
                CoroutineHelper.Start(BurnCard(card.CardId));
            }
        }

        //HARDCODED
        yield return new WaitForSecondsPauseable(1f);
        UpdateCardPositions();
    }

    private IEnumerator MoveCardTo(Vector3 targetPosition, Card card, float duration)
    {
        float elapsed = 0f;
        Vector3 startPosition = card.transform.position;
        Vector3 adjustedTargetPosition = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            card.transform.position = Vector3.Lerp(startPosition, adjustedTargetPosition, t);
            yield return null;
        }

        card.transform.position = adjustedTargetPosition;
    }


    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in CardsDictionary.Values)
        {
            card.State = newState;
        }
    }

    private void HandleClearTables()
    {
        foreach (var card in CardsDictionary.Values)
        {
            CoroutineHelper.Start(BurnCard(card.CardId));
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
