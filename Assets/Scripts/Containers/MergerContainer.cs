using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MergerContainer : CardContainerBase
{
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private ChestAnimation chestAnimation;
    [SerializeField] private Transform cardPositionTransform;
    
    private void Awake()
    {
        Application.targetFrameRate = 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        soAnimationEvents.OnToggleChestAnimation += HandleToggleCardsVisibility;
        soContainerEvents.OnMergeCards += HandleMergeCards;
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        
        soAnimationEvents.OnToggleChestAnimation -= HandleToggleCardsVisibility;
        soContainerEvents.OnMergeCards -= HandleMergeCards;
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
        var lastCard = CardsDictionary.LastOrDefault().Value;
        if (lastCard != null && lastCard.TokenType != CardData.TokenType.SingleDigit)
        {
            BurnCard(lastCard.CardId);
        }
        soAnimationEvents.RaiseToggleChestAnimation(false);
    }

    private void UpdateCardPositions()
    {
        int cardCount = CardsDictionary.Count;
        
        if (cardCount == 1)
        {
            var singleCard = CardsDictionary.Values.FirstOrDefault();
            if (singleCard != null)
            {
                singleCard.transform.localPosition = cardPositionTransform.localPosition;
                singleCard.transform.localRotation = cardPositionTransform.localRotation;
            }
        }
        else if (cardCount == 2)
        {
            var firstCard = CardsDictionary.Values.ElementAt(0);
            var secondCard = CardsDictionary.Values.ElementAt(1);

            var baseRotation = cardPositionTransform.localRotation;
            
            firstCard.transform.localPosition = cardPositionTransform.localPosition + new Vector3(0, 0, 0.75f);
            firstCard.transform.localRotation = baseRotation * Quaternion.Euler(0, -10, 0);
    
            secondCard.transform.localPosition = cardPositionTransform.localPosition + new Vector3(0.1f, 0, -0.75f);
            secondCard.transform.localRotation = baseRotation * Quaternion.Euler(0, 10, 0);
        }
    }

    private void PositionCard(Card card, Vector3 position, Quaternion rotation)
    {
        if (card == null) return;
        card.transform.position = position;
        card.transform.rotation = rotation;
    }

    private void HandleToggleCardsVisibility(bool isVisible)
    {
        CardsDictionary.Values
            .Where(card => card != null)
            .ToList()
            .ForEach(card => card.gameObject.SetActive(isVisible));
    }

    private void HandleMergeCards()
    {
        Debug.Log("MergeCall");
        CoroutineHelper.Start(MergeCards());
    }
    
    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in CardsDictionary.Values)
        {
            card.State = newState;
        }
    }

    private IEnumerator MergeCards()
    {
        if (CardsDictionary.Count != 2)
        {
            yield break;
        }
        
        soAnimationEvents.RaiseToggleChestAnimation(false);
        yield return new WaitWhile(() => chestAnimation!.IsMoving);
        
        var firstCard = CardsDictionary.Values.ElementAt(0);
        var secondCard = CardsDictionary.Values.ElementAt(1);
        
        firstCard.State = CardData.CardState.Normal;
        firstCard.Token += secondCard.Token;
        BurnCard(secondCard.CardId);
        
        UpdateCardPositions();
        
        soAnimationEvents.RaiseToggleChestAnimation(true);
        yield return new WaitWhile(() => chestAnimation!.IsMoving);
        
        var toKey = new ContainerKey(SelfContainerKey.OwnerType, CardContainerType.Hand);
        soCardEvents.RaiseCardMove(firstCard, SelfContainerKey, toKey);
    }
}