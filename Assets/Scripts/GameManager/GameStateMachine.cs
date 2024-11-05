using System.Collections;
using System.Collections.Generic;
using PlayerStates;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateMachine
{
    private GameStateBase _currentState;
    private GameStateBase _previousState;

    private Queue<GameStateBase> _stateQueue = new Queue<GameStateBase>();
    
    private bool _isTransitioning = false;
    
    public void ChangeState(GameStateBase newState)
    {
        _stateQueue.Enqueue(newState);
        
        if (!_isTransitioning)
        {
            _isTransitioning = true;

            CoroutineHelper.StartState(ProcessTransitionQueue());
        } 
    }

    private IEnumerator ProcessTransitionQueue()
    {
        while (_stateQueue.Count > 0)
        {
            GameStateBase newState = _stateQueue.Dequeue();
            yield return ChangeStateCoroutine(newState);
        }
        
        _isTransitioning = false;
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
    }

    public bool RevertToPreviousState()
    {
        if (_previousState != null && _previousState is not LookAroundState)
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
