using UnityEngine;

[CreateAssetMenu(fileName = "UI Events", menuName = "Events/UI Events")]
public class SoUIEvents : ScriptableObject
{
    public delegate void StartSliderHandler();
    public event StartSliderHandler OnStartSlider;
    
    public delegate void EndSliderHandler();
    public event EndSliderHandler OnEndSlider;

    public void RaiseStartSlider()
    {
        OnStartSlider?.Invoke();
    }

    public void RaiseEndSlider()
    {
        OnEndSlider?.Invoke();
    }
}