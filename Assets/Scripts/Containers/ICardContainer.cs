using System.Collections.Generic;
using UnityEngine;

public interface ICardContainer
{
    bool AddCard(Card card);
    bool RemoveCard(int cardId);
    List<Card> GetCards();
    Transform GetContainerTransform();
}