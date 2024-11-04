using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    private GameStateBase _currentState;
    private GameStateBase _previousState;

    private int _stateTransitionCoroutineId = -1;
    
    public void ChangeState(GameStateBase newState)
    {
        if (_stateTransitionCoroutineId != -1)
        {
            CoroutineHelper.StopState(_stateTransitionCoroutineId);
            _stateTransitionCoroutineId = -1;
        }
        
        _stateTransitionCoroutineId = CoroutineHelper.StartState(ChangeStateCoroutine(newState));
    }

    private IEnumerator ChangeStateCoroutine(GameStateBase newState)
    {
        if (_currentState != null)
        {
            yield return _currentState.Exit();
            _previousState = _currentState;
        }

        _currentState = newState;
        Debug.Log($"Game State Changed to {_currentState.GetType().Name}");
        yield return _currentState.Enter();

        _stateTransitionCoroutineId = -1;
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
