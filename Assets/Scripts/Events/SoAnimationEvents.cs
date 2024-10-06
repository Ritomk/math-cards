using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationEvents", menuName = "Events/Animation Events")]
public class SoAnimationEvents : ScriptableObject
{
    public delegate void CoinFlipAnimationHandler();
    public event CoinFlipAnimationHandler CoinFlipAnimation;
    
    public delegate void TimerAnimationHandler(float t);
    public event TimerAnimationHandler TimerAnimation;
    
    public delegate void TimerStartHandler(float duration, Action callback = null);
    
    public void RaiseCoinFlipAnimation() => CoinFlipAnimation?.Invoke();
    
    public void RaiseTimerAnimation(float t) => TimerAnimation?.Invoke(t);
}
