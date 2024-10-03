using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Deck : CardContainerBase
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float spacing = 0.2f;

    private void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            InstantiateCard(i.ToString());
        }
        InstantiateCard("+");
    }

    public void InstantiateCard(string token)
    {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        var card = cardObject.GetComponent<Card>();
        cardObject.name = $"Card {token}";
        card.Initialize(token, false, CardData.CardState.NonPickable);
        AddCard(card);
        UpdateCardsPositions();
    }
    
    // public override bool AddCard(Card card)
    // {
    //     base.AddCard(card);
    //     //card.transform.position = CalculateCardPosition(cardsDictionary.Count - 1);
    // }

    public Card DrawCard()
    {
        if (cardsDictionary.Count == 0)
        {
            Debug.LogWarning("No cards left to draw.");
            return null;
        }

        var card = cardsDictionary.Last();
        RemoveCard(card.Key);
        card.Value.IsTokenVisible = true;
        card.Value.State = CardData.CardState.Normal;
        return card.Value;
    }

    private Vector3 CalculateCardPosition(int cardIndex)
    {
        return transform.position + new Vector3(0, cardIndex * spacing, 0);
    }

    private void UpdateCardsPositions()
    {
        int index = 0;
        foreach (var card in cardsDictionary.Values)
        {
            card.transform.position = CalculateCardPosition(index);
            index++;
        }
    }
}