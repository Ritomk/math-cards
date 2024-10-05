using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


public enum CardContainerType
{
    Hand,
    Deck,
    AttackTable,
    DefenceTable
}

public class CardManager : MonoBehaviour
{
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private List<CardContainerBase> containerList;
    
    private Dictionary<CardContainerType, CardContainerBase> _cardContainers;

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
        _cardContainers = new Dictionary<CardContainerType, CardContainerBase>();

        foreach (var container in containerList)
        {
            if (container != null)
            {
                var containerType = container.ContainerType;

                if (!_cardContainers.TryAdd(containerType, container))
                {
                    Debug.LogWarning($"Duplicate container type detected: {containerType}");
                }
            }
        }
    }

    private void HandleCardMove(Card card, CardContainerType from, CardContainerType to)
    {
        if (_cardContainers.TryGetValue(from, out var fromContainer) &&
            _cardContainers.TryGetValue(to, out var toContainer))
        {
            if (!fromContainer.RemoveCard(card.CardId)) return;

            if (toContainer.AddCard(card))
            {
                Debug.Log($"Moved card {card.name} from {fromContainer.name} to {toContainer.name}");
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
        Debug.Log("Dupa");
        if (_cardContainers.TryGetValue(CardContainerType.Deck, out var deckContainer) &&
            _cardContainers.TryGetValue(CardContainerType.Hand, out var handContainer))
        {
            if (deckContainer is Deck deck)
            {
                Card drawnCard = deck.DrawCard();

                if (handContainer.AddCard(drawnCard))
                {
                    Debug.Log($"Card {drawnCard.name} drawn and added to hand.");
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
