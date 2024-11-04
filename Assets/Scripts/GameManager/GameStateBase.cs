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

    public virtual IEnumerator Enter() { yield break; }
    
    public virtual IEnumerator Exit() { yield break; }
    
    public virtual void Update() { }
}
