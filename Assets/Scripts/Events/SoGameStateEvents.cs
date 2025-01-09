using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateEvents", menuName = "Events/GameStateEvents")]
public class SoGameStateEvents : ScriptableObject
{
    #region Data
    
    private GameStateEnum _previousGameState;
    public GameStateEnum PreviousGameState => _previousGameState;
    
    [SerializeField] private GameStateEnum currentGameState;
    public GameStateEnum CurrentGameState => currentGameState;

    [SerializeField] private GameStateEnum runningGameState;
    public GameStateEnum RunningGameState => runningGameState;
    
    
    private PlayerStateEnum _previousPlayerState;
    public PlayerStateEnum PreviousPlayerState => _previousPlayerState;
    
    [SerializeField] private PlayerStateEnum currentPlayerState;
    public PlayerStateEnum CurrentPlayerState => currentPlayerState;
    
    [SerializeField] private PlayerStateEnum runningPlayerState;
    public PlayerStateEnum RunningPlayerState => runningPlayerState;
    
    #endregion

    
    #region Delegates

    public delegate void GameStateChangeHandler(GameStateEnum newState);
    public event GameStateChangeHandler OnGameStateChange;
    
    public delegate void PlayerStateChangeHandler(PlayerStateEnum newState);
    public event PlayerStateChangeHandler OnPlayerStateChange;

    public delegate void RevertGameStateHandler(out bool success);
    public event RevertGameStateHandler OnRevertGameState;
    
    public delegate void RevertPlayerStateHandler(out bool success);
    public event RevertPlayerStateHandler OnRevertPlayerState;
    
    public delegate void PauseGameHandler(bool paused);
    public event PauseGameHandler OnPauseGame;
    
    public delegate void EndRoundHandler(OwnerType owner);
    public event EndRoundHandler OnEndRound;
    
    public delegate void PlayerWonRoundHandler(bool wonByPlayer);
    public event PlayerWonRoundHandler OnPlayerWonRound;
    
    #endregion

    

    public void RaisePlayerStateChange(PlayerStateEnum newState)
    {
        _previousPlayerState = currentPlayerState;
        currentPlayerState = newState;
        OnPlayerStateChange?.Invoke(newState);
    }

    public void RaiseGameStateChange(GameStateEnum newState)
    {
        _previousGameState = currentGameState;
        currentGameState = newState;
        OnGameStateChange?.Invoke(newState);
    }

    public void RaiseOnRevertGameState()
    {
        if (OnRevertGameState != null)
        {
            OnRevertGameState(out bool shouldChangeState);

            if (shouldChangeState)
            {
                (_previousGameState, currentGameState) = (currentGameState, _previousGameState);
            }
        }
    }

    public void RaiseOnRevertPlayerState()
    {
        if (OnRevertPlayerState != null)
        {
            OnRevertPlayerState.Invoke(out bool shouldChangeState);

            if (shouldChangeState)
            {
                (_previousPlayerState, currentPlayerState) = (currentPlayerState, _previousPlayerState);
            }
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

    public void RaisePauseGame(bool paused)
    {
        OnPauseGame?.Invoke(paused);
    }

    public void UpdateCurrentGameState(GameStateEnum newState)
    {
        runningGameState = newState;
    }

    public void UpdateCurrentPlayerState(PlayerStateEnum newState)
    {
        runningPlayerState = newState;
    }

    public void RaiseEndRound(OwnerType owner)
    {
        OnEndRound?.Invoke(owner);
    }

    public void RaisePlayerWonRound(bool wonByPlayer)
    {
        OnPlayerWonRound?.Invoke(wonByPlayer);
    }
}

public enum GameStateEnum
{
    Default,
    Setup,
    BeginRound,
    PlayerTurn,
    OpponentTurn,
    SkipPlayerTurn,
    SkipOpponentTurn,
    EndRound,
    GameOver
}

public enum PlayerStateEnum
{
    Default,
    BeginRound,
    PlayerTurnIdle,
    CardPicked,
    CardPlacedTable,
    CardPlacedMerger,
    AllCardsPlaced,
    OpponentTurnIdle,
    LookAround,
    PauseGame
}
