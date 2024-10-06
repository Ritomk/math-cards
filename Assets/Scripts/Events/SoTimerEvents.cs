using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Timer Events", menuName = "Events/Timer Events")]
public class SoTimerEvents : ScriptableObject
{
    public delegate void StartTimerHandler(float duration);
    public event StartTimerHandler OnStartTimer;
    
    public delegate void StopTimerHandler();
    public event StopTimerHandler OnStopTimer;
    
    public delegate void TimerCompleteHandler();
    public event TimerCompleteHandler OnTimerComplete;

    public void RaiseStartTimer(float duration)
    {
        if (duration > 0f)
        {
            OnStartTimer?.Invoke(duration);
        }
        else
        {
            Debug.Log($"Timer event raised starting timer with duration {duration}");
        }
    }
    
    public void RaiseStopTimer() => OnStopTimer?.Invoke();
    
    public void RaiseCompleteTimer() => OnTimerComplete?.Invoke();
}
