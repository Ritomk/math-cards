using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private SoTimerEvents soTimerEvents;
    
    [SerializeField] private float timerDuration;
    
    private int _timerCoroutineId = -1;

    private void Awake()
    {
        if (soAnimationEvents == null || soTimerEvents == null)
        {
            Debug.LogError("Scriptable Objects not assigned", gameObject);
            this.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (soTimerEvents != null)
        {
            soTimerEvents.OnStartTimer += StartTimer;
            soTimerEvents.OnStopTimer += StopTimer;
        }
    }

    private void OnDisable()
    {
        if (soTimerEvents != null)
        {
            soTimerEvents.OnStartTimer -= StartTimer;
            soTimerEvents.OnStopTimer -= StopTimer;
        }
    }

    private void StartTimer(float duration)
    {
        if (_timerCoroutineId != -1)
        {
            CoroutineHelper.Stop(_timerCoroutineId);
            _timerCoroutineId = -1;
        }
        
        timerDuration = duration;
        _timerCoroutineId = CoroutineHelper.Start(TimerCoroutine());
    }

    private void StopTimer()
    {
        if (_timerCoroutineId != -1)
        {
            CoroutineHelper.Stop(_timerCoroutineId);
            _timerCoroutineId = -1;
            
            soAnimationEvents.RaiseTimerAnimation(0f);
        }
    }

    private IEnumerator TimerCoroutine()
    {
        float timeRemaining = timerDuration;
        float halfTime = timerDuration / 2.0f;

        while (timeRemaining > 0)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < halfTime)
            {
                soAnimationEvents.RaiseTimerAnimation(timeRemaining / halfTime);
            }
        }
        soTimerEvents.RaiseCompleteTimer();
    }
}
