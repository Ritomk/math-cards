using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationEvents", menuName = "Events/Animation Events")]
public class SoAnimationEvents : ScriptableObject
{
    public delegate void CoinFlipAnimationHandler();
    public event CoinFlipAnimationHandler OnCoinFlipAnimation;
    
    public delegate void TimerAnimationHandler(float t);
    public event TimerAnimationHandler OnTimerAnimation;
    
    public delegate void ToggleChestAnimationHandler(bool state);
    public event ToggleChestAnimationHandler OnToggleChestAnimation;
    
    public void RaiseCoinFlipAnimation() => OnCoinFlipAnimation?.Invoke();

    public void RaiseTimerAnimation(float t) => OnTimerAnimation?.Invoke(t);
    
    public void RaiseToggleChestAnimation(bool isOpen) => OnToggleChestAnimation?.Invoke(isOpen);
}
