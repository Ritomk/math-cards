using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private SoTimerEvents soTimerEvents;
    
    [SerializeField] private float timerDuration;
    
    private Coroutine _timerCoroutine;

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
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        
        timerDuration = duration;
        _timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private void StopTimer()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(TimerCoroutine());
            _timerCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine()
    {
        float timeRemaining = timerDuration;
        soAnimationEvents.RaiseTimerAnimation(timeRemaining / timerDuration);

        while (timeRemaining > 0)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
            soAnimationEvents.RaiseTimerAnimation(timeRemaining / timerDuration);
        }
        
        soAnimationEvents.RaiseTimerAnimation(0f);
        
        soTimerEvents.RaiseCompleteTimer();
    }
}
