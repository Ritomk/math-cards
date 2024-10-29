using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class CardContainerBase : MonoBehaviour, ICardContainer
{
    protected Dictionary<int, Card> cardsDictionary = new Dictionary<int, Card>();
    
    [field: SerializeField] public CardContainerType ContainerType { get; private set; }

    [SerializeField] protected int maxCardCount = 10;
    protected int currentCardCount = 0;

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
        if (cardsDictionary.TryAdd(cardId, card))
        {
            card.transform.parent = transform;
            card.ContainerType = this.ContainerType;
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
        if (cardsDictionary.Remove(cardId, out var card))
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
        if (cardsDictionary.Remove(cardId, out var card))
        {
            Destroy(card.gameObject);
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
        return new List<Card>(cardsDictionary.Values);
    }

    public virtual Transform GetContainerTransform()
    {
        return transform;
    }

    protected virtual void ValidateCardPlacement() { }
}