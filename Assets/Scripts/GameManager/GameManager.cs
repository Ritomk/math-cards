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
    
    [Header("Scriptable Objects")]
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    
    [Space]
    [Header("Player Scripts")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardHighlightController cardHighlightController;
    [SerializeField] private CardPickController cardPickController;
    [SerializeField] private CardSelectionController cardSelectionController;
    
    private void Awake()
    {
        _gameStateMachine = new GameStateMachine();
        _playerStateMachine = new GameStateMachine();
        
        soGameStateEvents.OnGameStateChange += HandleSoGameStateChange;
        soGameStateEvents.OnPlayerStateChange += HandlePlayerStateChange;
        soGameStateEvents.OnRevertGameState += HandleRevertSoGameState;
        soGameStateEvents.OnRevertPlayerState += HandleRevertPlayerState;
        
        
        //TODO: Dodaj do player turn
        soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.OpponentTurnIdle);
    }

    private void OnDestroy()
    {
        soGameStateEvents.OnGameStateChange -= HandleSoGameStateChange;
        soGameStateEvents.OnPlayerStateChange -= HandlePlayerStateChange;
        soGameStateEvents.OnRevertGameState -= HandleRevertSoGameState;
        soGameStateEvents.OnRevertPlayerState -= HandleRevertPlayerState;
    }

    private void Start()
    {
        soGameStateEvents.RaiseGameStateChange(GameStateEnum.Setup);
    }

    private void Update()
    {
        _gameStateMachine.Update();
    }

    private void HandleSoGameStateChange(GameStateEnum newState)
    {
        ChangeGameState(newState);
    }

    private void ChangeGameState(GameStateEnum newState)
    {
        switch (newState)
        {
            case GameStateEnum.Setup:
                _gameStateMachine.ChangeState(new GameStates.SetupState(_gameStateMachine, inputManager,
                    soGameStateEvents));
                break;
            case GameStateEnum.BeginRound:
                _gameStateMachine.ChangeState(new GameStates.BeginRoundState(_gameStateMachine, soGameStateEvents,
                    soCardEvents));
                break;
            case GameStateEnum.PlayerTurn:
                _gameStateMachine.ChangeState(new GameStates.PlayerTurnState(_gameStateMachine, soAnimationEvents,
                    soGameStateEvents ,cardPickController));
                break;
            case GameStateEnum.OpponentTurn:
                _gameStateMachine.ChangeState(new GameStates.OpponentTurnState(_gameStateMachine, soGameStateEvents));
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

    private bool HandleRevertSoGameState() =>  _gameStateMachine.RevertToPreviousState();

    private bool HandleRevertPlayerState() => _playerStateMachine.RevertToPreviousState();
}
