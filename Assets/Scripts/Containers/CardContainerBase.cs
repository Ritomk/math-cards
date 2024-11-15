using System.Collections.Generic;
using UnityEngine;

public abstract class CardContainerBase : MonoBehaviour
{
    [SerializeField] protected SoContainerEvents soContainerEvents;
    
    public Dictionary<int, Card> CardsDictionary { get; protected set; } = new Dictionary<int, Card>();

    [field: SerializeField] public ContainerKey SelfContainerKey { get; protected set; }

    [SerializeField] protected int maxCardCount = 10;
    protected int currentCardCount = 0;

    protected virtual void OnEnable()
    {
        soContainerEvents.OnGetCardData += HandleCardData;
    }

    protected virtual void OnDisable()
    {
        soContainerEvents.OnGetCardData -= HandleCardData;
    }

    protected virtual void HandleCardData(EnemyKnowledgeData data) { }

    public virtual bool AddCard(Card card)
    {
        if (card == null)
        {
            Debug.LogError($"Trying to add null card in the container {gameObject.name}", gameObject);
            return false;
        }

        if (currentCardCount >= maxCardCount)
        {
            Debug.LogWarning($"Cannot add card. Container {gameObject.name} has reached its maximum capacity of {maxCardCount} cards.", gameObject);
            return false;
        }
        
        var cardId = card.CardId;
        if (CardsDictionary.TryAdd(cardId, card))
        {
            card.transform.parent = transform;
            card.ContainerKey = SelfContainerKey;
            currentCardCount++;
            return true;
        }
        else
        {
            Debug.LogWarning($"Card with ID {cardId} already exists in the container {gameObject.name}.", gameObject);
            return false;
        }
    }

    public virtual bool RemoveCard(int cardId)
    {
        if (CardsDictionary.Remove(cardId, out var card))
        {
            currentCardCount--;
            return true;
        }
        else
        {
            Debug.LogWarning($"Card with ID {cardId} does not exist in the container {gameObject.name}.", gameObject);
            return false;
        }
    }

    public virtual bool BurnCard(int cardId)
    {
        if (CardsDictionary.Remove(cardId, out var card))
        {
            card.DestroyCard();
            currentCardCount--;
            return true;
        }
        else
        {
            Debug.LogWarning($"Card with ID {cardId} does not exist in the container {gameObject.name}.", gameObject);
            return false;
        }
    }

    public virtual List<Card> GetCards()
    {
        return new List<Card>(CardsDictionary.Values);
    }

    public virtual Transform GetContainerTransform()
    {
        return transform;
    }

    protected virtual void ValidateCardPlacement() { }
}