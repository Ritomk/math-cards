using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    private GameStateBase _currentState;
    private GameStateBase _previousState;

    public void ChangeState(GameStateBase newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit();
            _previousState = _currentState;
        }
        
        _currentState = newState;
        Debug.Log($"Game State Changed to {_currentState.GetType().Name}");
        _currentState.Enter();
    }

    public bool RevertToPreviousState()
    {
        if (_previousState != null)
        {
            ChangeState(_previousState);
            _previousState = null;
            return true;
        }
        else
        {
            Debug.LogWarning("No previous state to revert to");
            return false;
        }
    }

    public void Update()
    {
        _currentState?.Update();
    }
}
