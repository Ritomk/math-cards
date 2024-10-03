using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/CardSelectedEvent")]
public class CardSelectedEvent : ScriptableObject
{
    public Action<Card> OnCardSelected;

    public void Raise(Card card)
    {
        OnCardSelected?.Invoke(card);
    }
}

[CreateAssetMenu(menuName = "Events/CameraResetEvent")]
public class CameraResetEvent: ScriptableObject
{
    public Action<bool> OnCameraReset;

    public void Raise(bool reset)
    {
        OnCameraReset?.Invoke(reset);
    }
}