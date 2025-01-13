using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[CreateAssetMenu(fileName = "Container Events", menuName = "Events/Container Events")]
public class SoContainerEvents : ScriptableObject
{
    public delegate void ChangeCardsStateHandler(CardData.CardState newState);
    public event ChangeCardsStateHandler OnChangeCardsState;

    public delegate void EvaluateExpressionHandler();
    public event EvaluateExpressionHandler OnEvaluateExpression;
    
    public delegate void SendExpressionResultHandler(float result, ContainerKey containerKey);
    public event SendExpressionResultHandler OnSendExpressionResult;

    public delegate void ValidateCardPlacementHandler();
    public event ValidateCardPlacementHandler OnValidateCardPlacement;
    
    public delegate void MergeCardsHandler();
    public event MergeCardsHandler OnMergeCards;

    public delegate void GetCardDataHandler(EnemyKnowledgeData data);
    public event GetCardDataHandler OnGetCardData;
    
    public delegate void ClearTablesHandler();
    public event ClearTablesHandler OnClearTables;

    public void RaiseChangeCardsState(CardData.CardState newState)
    {
        OnChangeCardsState?.Invoke(newState);
    }
    
    public void RaiseEvaluateExpression()
    {
        OnEvaluateExpression?.Invoke();
    }

    public void RaiseSendExpressionResult(float result, ContainerKey containerKey)
    {
        OnSendExpressionResult?.Invoke(result, containerKey);
    }

    public void RaiseMergeCards()
    {
        OnMergeCards?.Invoke();
    }

    public void RaiseValidateCardPlacement()
    {
        OnValidateCardPlacement?.Invoke();
    }

    public EnemyKnowledgeData RaiseGetCardData()
    {
        EnemyKnowledgeData containersData = new EnemyKnowledgeData();
        OnGetCardData?.Invoke(containersData);
        return containersData;
    }

    public void RaiseClearTables()
    {
        OnClearTables?.Invoke();
    }
}