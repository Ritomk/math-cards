using System.Linq;
using UnityEngine;

public class DeckContainer : CardContainerBase, IDrawableContainer
{
    [Header("Deck Container Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float spacing = 0.2f;
    

    private void Start()
    {
        GenerateDeck(maxCardCount);
        ShuffleDeck();
    }

    protected override void OnEnable() => soContainerEvents.OnReshuffleCards += ShuffleDeck;
    
    protected override void OnDisable() => soContainerEvents.OnReshuffleCards -= ShuffleDeck;

    private void GenerateDeck(int amountOfCards)
    {
        var numberCount = (amountOfCards + 1) / 2;
        var operatorCount = amountOfCards / 2;
        
        int[] operandDistribution = new int[10];
        int targetOperandCount = numberCount / 10;
        int numbersAdded = 0, operatorsAdded = 0;
        
        for (int i = 0; i < amountOfCards; i++)
        {
            if (numbersAdded < numberCount && 
                (operatorsAdded >= operatorCount || i % 2 == 0))
            {
                int operandToAdd = -1;
                for (int j = 0; j < operandDistribution.Length; j++)
                {
                    if (operandDistribution[j] < targetOperandCount)
                    {
                        operandToAdd = j;
                        break;
                    }
                }

                if (operandToAdd == -1)
                {
                    operandToAdd = numbersAdded % 10;
                }

                InstantiateCard(operandToAdd);
                operandDistribution[operandToAdd]++;
                numbersAdded++;
            }
            else if (operatorsAdded < operatorCount)
            {
                var randomIndex = Random.Range(1, 5);
                TokenMapping.IntToFloatMap.TryGetValue(randomIndex, out var mappedIndex);
                InstantiateCard(mappedIndex);
                operatorsAdded++;
            }
        }
    }
    
    private void ShuffleDeck()
    {
        var cardList = CardsDictionary.ToList();

        for (int i = cardList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (cardList[i], cardList[randomIndex]) = (cardList[randomIndex], cardList[i]);
        }

        CardsDictionary.Clear();
        foreach (var cardEntry in cardList)
        {
            CardsDictionary.Add(cardEntry.Key, cardEntry.Value);
        }

        UpdateCardsPositions();
        Debug.Log("Deck shuffled!");
    }

    private void InstantiateCard(float token)
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