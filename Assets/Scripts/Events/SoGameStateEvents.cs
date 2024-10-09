using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameStateEvents", menuName = "Events/GameStateEvents")]
public class SoGameStateEvents : ScriptableObject
{
    #region Data
    
    private GameStateEnum _previousGameState;
    public GameStateEnum PreviousGameState => _previousGameState;
    
    [SerializeField] private GameStateEnum currentGameState;
    public GameStateEnum CurrentGameState => currentGameState;
    
    
    private PlayerStateEnum _previousPlayerState;
    public PlayerStateEnum PreviousPlayerState => _previousPlayerState;
    
    [SerializeField] private PlayerStateEnum currentPlayerState;
    public PlayerStateEnum CurrentPlayerState => currentPlayerState;
    
    #endregion

    
    #region Delegates

    public delegate void GameStateChangeHandler(GameStateEnum newState);
    public event GameStateChangeHandler OnGameStateChange;
    
    public delegate void PlayerStateChangeHandler(PlayerStateEnum newState);
    public event PlayerStateChangeHandler OnPlayerStateChange;

    public delegate bool RevertGameStateHandler();
    public event RevertGameStateHandler OnRevertGameState;
    
    public delegate bool RevertPlayerStateHandler();
    public event RevertPlayerStateHandler OnRevertPlayerState;
    
    #endregion

    

    public void RaiseOnPlayerStateChange(PlayerStateEnum newState)
    {
        _previousPlayerState = currentPlayerState;
        currentPlayerState = newState;
        OnPlayerStateChange?.Invoke(currentPlayerState);
    }

    public void RaiseGameStateChange(GameStateEnum newState)
    {
        _previousGameState = currentGameState;
        currentGameState = newState;
        OnGameStateChange?.Invoke(newState);
    }

    public void RaiseOnRevertGameState()
    {
        bool shouldChangeState = true;
        
        if (OnRevertGameState != null)
        {
            foreach (var @delegate in OnRevertGameState.GetInvocationList())
            {
                var handler = (RevertGameStateHandler)@delegate;
                bool response = handler();
                if (!response)
                {
                    shouldChangeState = false;
                    break;
                }
            }
        }
        
        if (shouldChangeState)
        {
            (_previousGameState, currentGameState) = (currentGameState, _previousGameState);
        }
    }

    public void RaiseOnRevertPlayerState()
    {
        bool shouldChangeState = true;

        if (OnRevertPlayerState != null)
        {
            foreach (var @delegate in OnRevertPlayerState.GetInvocationList())
            {
                var handler = (RevertPlayerStateHandler)@delegate;
                bool response = handler();
                if (!response)
                {
                    shouldChangeState = false;
                    break;
                }
            }
        }
        
        if (shouldChangeState)
        {
            (_previousPlayerState, currentPlayerState) = (currentPlayerState, _previousPlayerState);
        }
    }

    public void RaiseNextState()
    {
        int nextStateIndex = (int)currentGameState + 1;

        if (nextStateIndex >= Enum.GetNames(typeof(GameStateEnum)).Length)
        {
            Debug.LogWarning($"There so no more states are available for the current state to move: {currentGameState}");
            return;
        }

        ++currentGameState;
        OnGameStateChange?.Invoke(currentGameState);
    }
}

public enum GameStateEnum
{
    Setup,
    BeginRound,
    PlayerTurn,
    OpponentTurn,
    EndRound,
}

public enum PlayerStateEnum
{
    PlayerTurnIdle,
    CardPicked,
    CardPlaced,
    OpponentTurnIdle,
    LookAround,
    PauseGame
}
