using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    #region Variables
    private GameStateMachine _gameStateMachine;
    private GameStateMachine _playerStateMachine;

    private Dictionary<GameStateEnum, GameStateBase> _gameStates;
    private Dictionary<PlayerStateEnum, GameStateBase> _playerStates;
    
    [Header("Scriptable Objects")]
    [SerializeField] private SoUniversalInputEvents soUniversalInputEvents;
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private SoTimerEvents soTimerEvents;
    
    [Space]
    [Header("Player Gameplay Scripts")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardHighlightController cardHighlightController;
    [SerializeField] private CardPickController cardPickController;
    [SerializeField] private CardSelectionController cardSelectionController;
    
    [Space]
    [Header("Player Menu Scripts")]
    [SerializeField] private InputUIManager inputUIManager;
    
    #endregion
    
    private void Awake()
    {
        _gameStateMachine = new GameStateMachine();
        _playerStateMachine = new GameStateMachine();
        
        soGameStateEvents.OnGameStateChange += HandleSoGameStateChange;
        soGameStateEvents.OnPlayerStateChange += HandlePlayerStateChange;
        soGameStateEvents.OnRevertGameState += HandleRevertSoGameState;
        soGameStateEvents.OnRevertPlayerState += HandleRevertPlayerState;


        _gameStates = new Dictionary<GameStateEnum, GameStateBase>
        {
            {
                GameStateEnum.Setup, new GameStates.SetupState(_gameStateMachine, inputManager,
                    soGameStateEvents)
            },
            {
                GameStateEnum.BeginRound, new GameStates.BeginRoundState(_gameStateMachine, soGameStateEvents,
                    soCardEvents)
            },
            {
                GameStateEnum.PlayerTurn, new GameStates.PlayerTurnState(_gameStateMachine, soAnimationEvents,
                    soGameStateEvents, soTimerEvents, cardPickController)
            },
            { GameStateEnum.OpponentTurn, new GameStates.OpponentTurnState(_gameStateMachine, soGameStateEvents) }
        };

        _playerStates = new Dictionary<PlayerStateEnum, GameStateBase>
        {
            {
                PlayerStateEnum.PlayerTurnIdle, new PlayerStates.PlayerTurnIdleState(_playerStateMachine,
                    cardHighlightController, cardPickController)
            },
            {
                PlayerStateEnum.CardPicked, new PlayerStates.PlayerPickedCardState(_playerStateMachine,
                    cardHighlightController)
            },
            {
                PlayerStateEnum.CardPlaced, new PlayerStates.PlayerPlacedCardState(_playerStateMachine,
                    cardHighlightController)
            },
            {
                PlayerStateEnum.OpponentTurnIdle, new PlayerStates.OpponentTurnIdleState(_playerStateMachine,
                    soCardEvents, cardPickController)
            },
            {
                PlayerStateEnum.LookAround, new PlayerStates.LookAroundState(_playerStateMachine,
                    cardSelectionController)
            },
            {
                PlayerStateEnum.PauseGame, new PlayerStates.PauseGameState(_playerStateMachine,
                    inputManager, inputUIManager, soUniversalInputEvents, soCardEvents)
            }
        };
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
        if (_gameStates.TryGetValue(newState, out GameStateBase gameState))
        {
            _gameStateMachine.ChangeState(gameState);
        }
        else
        {
            Debug.LogWarning($"Game state {newState} not found");
        }
    }
    
    private void HandlePlayerStateChange(PlayerStateEnum newState)
    {
        ChangePlayerState(newState);
    }

    private void ChangePlayerState(PlayerStateEnum newState)
    {
        if (_playerStates.TryGetValue(newState, out GameStateBase state))
        {
            _playerStateMachine.ChangeState(state);
        }
        else
        {
            Debug.LogWarning($"Player state {newState} not found.");
        }
    }

    private bool HandleRevertSoGameState() =>  _gameStateMachine.RevertToPreviousState();

    private bool HandleRevertPlayerState() => _playerStateMachine.RevertToPreviousState();
}
