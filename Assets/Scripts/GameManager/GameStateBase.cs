using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateBase
{
    protected GameStateMachine stateMachine;
    
    public GameStateBase(GameStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    
    public virtual void Enter() { }
    
    public virtual void Exit() { }
    
    public virtual void Update() { }
}
