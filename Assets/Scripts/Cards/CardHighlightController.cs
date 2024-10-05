using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardSelectionController))]
public class CardHighlightController : MonoBehaviour
{
    [SerializeField] private SoCardEvents soCardEvents;

    [SerializeField] private Card selectedCard;
    [SerializeField] private Card _highlightedCard;
    

    private void OnEnable()
    {
        if (soCardEvents != null)
        {
            soCardEvents.OnCardSelected += HighlightCard;
        }
    }

    private void OnDisable()
    {
        if(soCardEvents != null)
        {
            soCardEvents.OnCardSelected -= HighlightCard;
        }
    }

    private void HighlightCard(Card card)
    {
        if (card != null && card.CompareTag("Card"))
        {
            if(_highlightedCard == null || _highlightedCard?.State == CardData.CardState.Normal
               || _highlightedCard != card)
            {
                UpdateSelectedCard(card);
            }
        }
        else
        {
            ResetSelectedCard();
        }
    }

    private void UpdateSelectedCard(Card card)
    {

        if (_highlightedCard != null)
        {
            HighlightCard(_highlightedCard, false);
        }
        
        if (card.ContainerType == CardContainerType.Hand)
        {
            selectedCard.Token = card.Token;

            if (!selectedCard.gameObject.activeInHierarchy)
            {
                selectedCard.gameObject.SetActive(true);
            }
        }
        
        _highlightedCard = card;
        HighlightCard(card, true);
    }

    private void ResetSelectedCard()
    {
        if (selectedCard != null && selectedCard.gameObject.activeInHierarchy)
        {
            selectedCard.gameObject.SetActive(false);
        }

        if (_highlightedCard != null)
        {
            HighlightCard(_highlightedCard, false);
            _highlightedCard = null;
        }
    }

    private void HighlightCard(Card card, bool highlight)
    {
        card.State = highlight ? CardData.CardState.Highlighted : CardData.CardState.Normal;
    }
}
