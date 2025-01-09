using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private List<CardContainerBase> containerList;
    
    [Space]
    [SerializeField] private OwnerType ownerType;
    private Dictionary<ContainerKey, CardContainerBase> _cardContainers;

    private void Awake()
    {
        InitializeCardContainers();
    }

    private void OnEnable()
    {
        if (soCardEvents != null)
        {
            soCardEvents.OnCardMove += HandleCardMove;
            soCardEvents.OnCardDraw += HandleCardDraw;
        }
    }

    private void OnDisable()
    {
        if (soCardEvents != null)
        {
            soCardEvents.OnCardMove -= HandleCardMove;
            soCardEvents.OnCardDraw -= HandleCardDraw;
        }
    }

    private void InitializeCardContainers()
    {
        _cardContainers = new Dictionary<ContainerKey, CardContainerBase>();

        foreach (var container in containerList)
        {
            if (container != null)
            {
                if (!_cardContainers.TryAdd(container.SelfContainerKey, container))
                {
                    Debug.LogWarning($"Duplicate container detected: {container.SelfContainerKey.OwnerType} - {container.SelfContainerKey.ContainerType}");
                }
            }
        }
    }

    private void HandleCardMove(Card card, ContainerKey from, ContainerKey to, out bool success)
    {
        success = false;

        if (from.Equals(to))
        {
            success = true;
            return;
        }
        
        if (_cardContainers.TryGetValue(from, out var fromContainer) &&
            _cardContainers.TryGetValue(to, out var toContainer))
        {
            if (!fromContainer.RemoveCard(card.CardId)) return;

            if (toContainer.AddCard(card))
            {
                Debug.Log($"Moved card {card.name} from {fromContainer.name} to {toContainer.name}");
                success = true;
            }
            else
            {
                fromContainer.AddCard(card);
                Debug.LogError($"Failed to add card {card.name} to {toContainer.name}. Rolled back to {fromContainer.name}.");
            }
        }
        else
        {
            Debug.LogWarning($"Failed to move card. Invalid container(s): {from}, {to}");
        }
    }

    private void HandleCardDraw(bool toHand, out bool success)
    {
        success = false;
        
        var fromKey = toHand
            ? new ContainerKey(ownerType, CardContainerType.Deck)
            : new ContainerKey(ownerType, CardContainerType.Hand);

        var toKey = toHand
            ? new ContainerKey(ownerType, CardContainerType.Hand)
            : new ContainerKey(ownerType, CardContainerType.Deck);
        
        if (_cardContainers.TryGetValue(fromKey, out var fromContainer) &&
            _cardContainers.TryGetValue(toKey, out var toContainer))
        {
            Card drawnCard = null;

            if (fromContainer is IDrawableContainer drawableContainer)
            {
                drawnCard = drawableContainer.DrawCard();
            }
            else
            {
                Debug.LogWarning("Container does not implement IDrawableContainer, cannot draw a card.");
            }

            if (!drawnCard) return;

            if (toContainer.AddCard(drawnCard))
            {
                success = true;
            }
            else
            {
                fromContainer.AddCard(drawnCard);
                Debug.LogError($"Failed to add card {drawnCard.name} to {toContainer.name}. " +
                               $"Returned it to {fromContainer.name}.");
            }
        }
        else
        {
            Debug.LogWarning("Either Deck or Hand container not found. Cannot draw a card.");
        }
    }
}
