using System.Collections;
using System.Collections.Generic;
using PlayerStates;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine<TStateEnum>
{
    private StateBase<TStateEnum> _currentState;
    private StateBase<TStateEnum> _previousState;

    private Queue<StateBase<TStateEnum>> _stateQueue = new Queue<StateBase<TStateEnum>>();
    
    public delegate void GameStateChangeHandler(StateBase<TStateEnum> newState);
    public event GameStateChangeHandler OnStateChanged;
    
    private bool _isTransitioning = false;
    
    public void ChangeState(StateBase<TStateEnum> newState)
    {
        _stateQueue.Enqueue(newState);
        
        if (!_isTransitioning)
        {
            _isTransitioning = true;

            CoroutineHelper.StartState(ProcessTransitionQueue(), newState.GetType().ToString());
        } 
    }

    private IEnumerator ProcessTransitionQueue()
    {
        while (_stateQueue.Count > 0)
        {
            StateBase<TStateEnum> newState = _stateQueue.Dequeue();
            yield return ChangeStateCoroutine(newState);
        }
        
        _isTransitioning = false;
    }
    
    private IEnumerator ChangeStateCoroutine(StateBase<TStateEnum> newState)
    {
        if (_currentState != null)
        {
            yield return _currentState.Exit();
            _previousState = _currentState;
        }

        _currentState = newState;
        OnStateChanged?.Invoke(_currentState);
        
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
