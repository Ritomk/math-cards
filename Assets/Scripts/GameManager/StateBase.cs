using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StateBase<TStateEnum>
{
    public abstract TStateEnum StateType { get; }
    
    protected StateMachine<TStateEnum> stateMachine;
    
    public StateBase(StateMachine<TStateEnum> stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual IEnumerator Enter() { yield break; }
    
    public virtual IEnumerator Exit() { yield break; }
    
    public virtual void Update() { }
}

public abstract class GameStateBase : StateBase<GameStateEnum>
{
    public GameStateBase(StateMachine<GameStateEnum> stateMachine) : base(stateMachine) { }
}

public abstract class PlayerStateBase : StateBase<PlayerStateEnum>
{
    public PlayerStateBase(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine) { }
}
