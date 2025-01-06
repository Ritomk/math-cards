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

    private void HandleCardDraw()
    {
        var deckKey = new ContainerKey(ownerType, CardContainerType.Deck);
        var handKey = new ContainerKey(ownerType, CardContainerType.Hand);
        
        if (_cardContainers.TryGetValue(deckKey, out var deckContainer) &&
            _cardContainers.TryGetValue(handKey, out var handContainer))
        {
            if (deckContainer is DeckContainer deck)
            {
                Card drawnCard = deck.DrawCard();

                if (handContainer.AddCard(drawnCard))
                {
                    //Debug.Log($"Card {drawnCard.name} drawn and added to hand.");
                }
                else
                {
                    deck.AddCard(drawnCard);
                    Debug.LogError($"Failed to add card {drawnCard.name} to hand. Returned it to the deck.");
                }
            }
            else
            {
                Debug.LogWarning("Deck container is not of type 'Deck'. Cannot draw a card.");
            }
        }
        else
        {
            Debug.LogWarning("Deck or hand container not found. Cannot draw a card.");
        }
    }
}
