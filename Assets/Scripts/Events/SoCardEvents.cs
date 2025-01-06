using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEvents", menuName = "Events/Card Events")]
public class SoCardEvents : ScriptableObject
{
    public delegate void CardMoveHandler(Card card, ContainerKey fromContainer, ContainerKey toContainer, out bool success);
    public event CardMoveHandler OnCardMove;

    public delegate void CardDrawHandler();
    public event CardDrawHandler OnCardDraw;

    public delegate void CardSelectedHandler(Card card);
    public event CardSelectedHandler OnCardSelected;

    public delegate void CardSelectionResetHandler();
    public event CardSelectionResetHandler OnCardSelectionReset;
    
    public bool RaiseCardMove(Card card, ContainerKey fromContainer, ContainerKey toContainer)
    {
        if (OnCardMove != null)
        {
            OnCardMove.Invoke(card, fromContainer, toContainer, out bool success);
            return success;
        }
        return false;
    }

    public void RaiseCardDraw()
    {
        OnCardDraw?.Invoke();
    }

    public void RaiseCardSelected(Card card)
    {
        OnCardSelected?.Invoke(card);
    }

    public void RaiseCardSelectionReset()
    {
        OnCardSelectionReset?.Invoke();
    }
}