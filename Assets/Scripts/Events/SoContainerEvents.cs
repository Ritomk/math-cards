using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Container Events", menuName = "Events/Container Events")]
public class SoContainerEvents : ScriptableObject
{
    public delegate void ChangeCardsStateHandler(CardData.CardState newState);
    public event ChangeCardsStateHandler OnChangeCardsState;

    public delegate float EvaluateExpressionHandler();
    public event EvaluateExpressionHandler OnEvaluateExpression;

    public delegate void ValidateCardPlacementHandler();
    public event ValidateCardPlacementHandler OnValidateCardPlacement;
    
    public delegate void MergeCardsHandler();
    public event MergeCardsHandler OnMergeCards;

    public delegate void GetCardDataHandler(EnemyKnowledgeData data);
    public event GetCardDataHandler OnGetCardData;

    public void RaiseChangeCardsState(CardData.CardState newState)
    {
        OnChangeCardsState?.Invoke(newState);
    }

    //TODO: Checking who's subscriber and what to do with value
    public void RaiseEvaluateExpression()
    {
        if (OnEvaluateExpression != null)
        {
            foreach (var @delegate in OnEvaluateExpression.GetInvocationList())
            {
                var handler = (EvaluateExpressionHandler)@delegate;
                float expressionValue = handler();
            }
        }
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
}