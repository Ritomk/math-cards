using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TStateEnum>
{
    private StateBase<TStateEnum> _currentState;

    private Queue<StateBase<TStateEnum>> _stateQueue = new Queue<StateBase<TStateEnum>>();
    
    private const int MAX_STATE_HISTORY_SIZE = 10;
    private List<StateBase<TStateEnum>> _stateHistory = new List<StateBase<TStateEnum>>();
    
    public delegate void GameStateChangeHandler(StateBase<TStateEnum> newState);
    public event GameStateChangeHandler OnStateChanged;
    
    private bool _isTransitioning = false;
    
    public void ChangeState(StateBase<TStateEnum> newState)
    {
        if (_currentState == newState)
        {
            Debug.LogWarning($"Attempted to change to the same state: {newState.GetType().Name}");
            return;
        }
        
        // Prevent consecutive duplicate states in the queue
        if (_stateQueue.Count > 0 && _stateQueue.Peek() == newState)
        {
            Debug.LogWarning($"Duplicate state in queue: {newState.GetType().Name}");
            return;
        }
        
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
            
            AddToHistory(_currentState);
        }

        _currentState = newState;
        OnStateChanged?.Invoke(_currentState);
        
        Debug.Log($"Game State Changed to {_currentState.GetType().Name}");
        yield return _currentState.Enter();
    }

    private void AddToHistory(StateBase<TStateEnum> state)
    {
        if(_stateHistory.Count > 0 && _stateHistory[^1] == state)
            return;

        if (_stateHistory.Count >= MAX_STATE_HISTORY_SIZE)
        {
            _stateHistory.RemoveAt(0);
        }
        _stateHistory.Add(state);
    }

    public bool RevertToPreviousState()
    {
        // if (_previousState != null && _previousState is not LookAroundState)
        // {
        //     ChangeState(_previousState);
        //     _previousState = null;
        //     return true;
        // }
        if (_stateHistory.Count > 0)
        {
            var previousState = _stateHistory[^1];
            _stateHistory.RemoveAt(_stateHistory.Count - 1);
            Debug.Log("State history: " + string.Join(" ", _stateHistory));
            ChangeState(previousState);
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
