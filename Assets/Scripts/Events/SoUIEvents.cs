using UnityEngine;

[CreateAssetMenu(fileName = "UI Events", menuName = "Events/UI Events")]
public class SoUIEvents : ScriptableObject
{
    public delegate void StartSliderHandler();
    public event StartSliderHandler OnStartSlider;
    
    public delegate void EndSliderHandler();
    public event EndSliderHandler OnEndSlider;
    
    public delegate void SetCrystalsAmountHandler(int playerAmount, int opponentAmount);
    public event SetCrystalsAmountHandler OnSetCrystalsAmount;
    
    public delegate void EndGameHandler(bool playerWon);
    public event EndGameHandler OnEndGame;

    public void RaiseStartSlider()
    {
        OnStartSlider?.Invoke();
    }

    public void RaiseEndSlider()
    {
        OnEndSlider?.Invoke();
    }

    public void RaiseSetCrystalsAmount(int playerAmount, int opponentAmount)
    {
        OnSetCrystalsAmount?.Invoke(playerAmount, opponentAmount);
    }

    public void RaiseEndGame(bool playerWon)
    {
        OnEndGame?.Invoke(playerWon);
    }
}