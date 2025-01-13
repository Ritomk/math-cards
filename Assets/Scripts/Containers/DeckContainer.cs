using System.Linq;
using UnityEngine;

public class DeckContainer : CardContainerBase, IDrawableContainer
{
    [Header("Deck Container Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float spacing = 0.2f;

    private void Start()
    {
        for (int i = 0; i < maxCardCount - (maxCardCount / 2); i++)
        {
            InstantiateCard(i % 9);
            if (i % 2 == 0)
            {
                var randomIndex = Random.Range(101, 105);
                InstantiateCard(randomIndex);
            }
        }
        UpdateCardsPositions();
    }

    public void InstantiateCard(int token)
    {
        GameObject cardObject = Instantiate(cardPrefab, transform);
        var card = cardObject.GetComponent<Card>();
        cardObject.name = $"Card {token}";
        card.Initialize(token, false, CardData.CardState.NonPickable, 0);
        AddCard(card);
    }

    public Card DrawCard()
    {
        if (CardsDictionary.Count == 0)
        {
            Debug.LogWarning("No cards left to draw.");
            return null;
        }

        var card = CardsDictionary.Last();
        RemoveCard(card.Key);
        card.Value.State = CardData.CardState.Normal;
        return card.Value;
    }

    public override bool AddCard(Card card)
    {
        bool result = base.AddCard(card);
        if (result)
        {
            UpdateCardsPositions();
            card.transform.rotation = Quaternion.Euler(0, 0, 0);
            card.State = CardData.CardState.NonPickable;
        }
        
        return result;
    }

    private Vector3 CalculateCardPosition(int cardIndex)
    {
        return transform.position + new Vector3(0, cardIndex * spacing, 0);
    }

    private void UpdateCardsPositions()
    {
        int index = 0;
        foreach (var card in CardsDictionary.Values)
        {
            card.transform.position = CalculateCardPosition(index);
            index++;
        }
    }

    protected override void HandleCardData(EnemyKnowledgeData data)
    {
        switch (SelfContainerKey.OwnerType)
        {
            case OwnerType.Enemy:
                data.selfDeckCardsCount = CardsDictionary.Count;
                break;
            case OwnerType.Player:
                data.playerDeckCardsCount = CardsDictionary.Count;
                break;
        }
    }
}