using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    private GameStateMachine _gameStateMachine;
    private GameStateMachine _playerStateMachine;
    
    [SerializeField] private SoGameStateEvents gameStateEvents;
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardHighlightController cardHighlightController;
    [SerializeField] private CardPickController cardPickController;
    [SerializeField] private CardSelectionController cardSelectionController;
    
    private void Awake()
    {
        _gameStateMachine = new GameStateMachine();
        _playerStateMachine = new GameStateMachine();
        
        gameStateEvents.OnGameStateChange += HandleGameStateChange;
        gameStateEvents.OnPlayerStateChange += HandlePlayerStateChange;
        gameStateEvents.OnRevertGameState += HandleRevertGameState;
        gameStateEvents.OnRevertPlayerState += HandleRevertPlayerState;
        
        gameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        
        //TODO: Dodaj do player turn
        gameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
    }

    private void OnDestroy()
    {
        gameStateEvents.OnGameStateChange -= HandleGameStateChange;
        gameStateEvents.OnPlayerStateChange -= HandlePlayerStateChange;
        gameStateEvents.OnRevertGameState -= HandleRevertGameState;
        gameStateEvents.OnRevertPlayerState -= HandleRevertPlayerState;
    }

    private void Update()
    {
        _gameStateMachine.Update();
    }

    private void HandleGameStateChange(GameStateEnum newState)
    {
        ChangeGameState(newState);
    }

    private void ChangeGameState(GameStateEnum newState)
    {
        switch (newState)
        {
            case GameStateEnum.Setup:
                _gameStateMachine.ChangeState(new GameStates.SetupState(_gameStateMachine, inputManager));
                break;
            case GameStateEnum.BeginRound:
                break;
            case GameStateEnum.PlayerTurn:
                _gameStateMachine.ChangeState(new GameStates.PlayerTurnState(_gameStateMachine, cardPickController));
                break;
            case GameStateEnum.OpponentTurn:
                break;
            default:
                Debug.LogWarning($"Case not implemented: {newState}");
                break;
        }
    }
    
    private void HandlePlayerStateChange(PlayerStateEnum newState)
    {
        ChangePlayerState(newState);
    }

    private void ChangePlayerState(PlayerStateEnum newState)
    {
        switch (newState)
        {
            case PlayerStateEnum.PlayerTurnIdle:
                _playerStateMachine.ChangeState(new PlayerStates.PlayerTurnIdleState(_playerStateMachine, 
                    cardHighlightController, cardPickController));
                break;
            case PlayerStateEnum.CardPicked:
                _playerStateMachine.ChangeState(new PlayerStates.PlayerPickedCardState(_playerStateMachine, 
                    cardHighlightController));
                break;
            case PlayerStateEnum.CardPlaced:
                _playerStateMachine.ChangeState(new PlayerStates.PlayerPlacedCardState(_playerStateMachine, 
                    cardHighlightController));
                break;
            case PlayerStateEnum.OpponentTurnIdle:
                break;
            case PlayerStateEnum.LookAround:
                _playerStateMachine.ChangeState(new PlayerStates.LookAroundState(_playerStateMachine,
                    cardSelectionController));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    private bool HandleRevertGameState() =>  _gameStateMachine.RevertToPreviousState();

    private bool HandleRevertPlayerState() => _playerStateMachine.RevertToPreviousState();
}
