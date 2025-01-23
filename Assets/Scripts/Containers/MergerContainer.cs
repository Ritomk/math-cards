using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MergerContainer : CardContainerBase
{
    [Header("Merger Container Settings")]
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
        
        soAnimationEvents.OnToggleChestAnimation += HandleToggleChestAnimation;
        soContainerEvents.OnMergeCards += HandleMergeCards;
        soContainerEvents.OnChangeCardsState += HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement += ValidateCardPlacement;
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        
        soAnimationEvents.OnToggleChestAnimation -= HandleToggleChestAnimation;
        soContainerEvents.OnMergeCards -= HandleMergeCards;
        soContainerEvents.OnChangeCardsState -= HandleChangeCardsState;
        soContainerEvents.OnValidateCardPlacement -= ValidateCardPlacement;
    }

    public override bool AddCard(Card card)
    {
        bool result = base.AddCard(card);
        if (result)
        {
            if (SelfContainerKey.OwnerType == OwnerType.Enemy)
            {
                card.IsTokenVisible = false;
            }
            
            UpdateCardPositions();
        }
        
        return result;
    }

    protected override void ValidateCardPlacement()
    {
        var burnDuration = 0f;
        var lastCard = CardsDictionary.LastOrDefault().Value;
        if (lastCard != null && lastCard.TokenType != CardData.TokenType.SingleDigit)
        {
            burnDuration = 1f;
            CoroutineHelper.Start(BurnCard(lastCard.CardId));
        }
        
        CoroutineHelper.Start(CloseLidAfterBurn(burnDuration));
    }

    private IEnumerator CloseLidAfterBurn(float burnDuration)
    {
        yield return new WaitForSeconds(burnDuration);
        HandleToggleChestAnimation(SelfContainerKey.OwnerType, false);
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

    private void HandleToggleChestAnimation(OwnerType ownerType, bool isOpen)
    {
        if(ownerType != OwnerType.Any &&
           SelfContainerKey.OwnerType != ownerType) return;
        
        Debug.Log("ToggleChest Called");
        chestAnimation.ToggleLead(isOpen);
        
        CardsDictionary.Values
            .Where(card => card != null)
            .ToList()
            .ForEach(card => card.gameObject.SetActive(isOpen));
    }

    private void HandleMergeCards() => CoroutineHelper.Start(MergeCards());
    
    private void HandleChangeCardsState(CardData.CardState newState)
    {
        foreach (var card in CardsDictionary.Values)
        {
            card.State = newState;
            
            if (SelfContainerKey.OwnerType == OwnerType.Enemy)
            {
                card.IsTokenVisible = false;
            }
        }
    }

    private IEnumerator MergeCards()
    {
        if (CardsDictionary.Count != 2)
        {
            yield break;
        }
        
        HandleToggleChestAnimation(SelfContainerKey.OwnerType, false);
        yield return new WaitWhile(() => chestAnimation!.IsMoving);
        
        var firstCard = CardsDictionary.Values.ElementAt(0);
        var secondCard = CardsDictionary.Values.ElementAt(1);
        
        //firstCard.State = CardData.CardState.Normal;
        firstCard.Token = (firstCard.Token * 10) + secondCard.Token;
        
        RemoveCard(secondCard.CardId);
        Destroy(secondCard.gameObject);
        
        UpdateCardPositions();
        
        HandleToggleChestAnimation(SelfContainerKey.OwnerType, true);
        yield return new WaitWhile(() => chestAnimation!.IsMoving);
        firstCard.State = CardData.CardState.Normal;
        
        var toKey = new ContainerKey(SelfContainerKey.OwnerType, CardContainerType.Hand);
        soCardEvents.RaiseCardMove(firstCard, SelfContainerKey, toKey);
    }

    protected override void HandleCardData(EnemyKnowledgeData data)
    {
        switch (SelfContainerKey.OwnerType)
        {
            case OwnerType.Enemy:
                data.selfMergerList = CardsDictionary.Select(x => x.Value.Token).ToList();
                break;
            case OwnerType.Player:
                data.playerMergerCardsCount = CardsDictionary.Count;
                break;
        }
    }
}