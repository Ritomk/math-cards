using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationEvents", menuName = "Events/Animation Events")]
public class SoAnimationEvents : ScriptableObject
{
    public delegate void CoinFlipAnimationHandler();
    public event CoinFlipAnimationHandler CoinFlipAnimation;
    
    public delegate void TimerAnimationHandler();
    public event TimerAnimationHandler TimerAnimation;
    
    public void RaiseCoinFlipAnimation() => CoinFlipAnimation?.Invoke();
    
    public void RaiseTimerAnimation() => TimerAnimation?.Invoke();
}
