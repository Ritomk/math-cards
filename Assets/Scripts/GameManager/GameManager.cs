using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.BehaviourTrees;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    #region Variables
    private StateMachine<GameStateEnum> _gameStateMachine;
    private StateMachine<PlayerStateEnum> _playerStateMachine;

    private Dictionary<GameStateEnum, GameStateBase> _gameStates;
    private Dictionary<PlayerStateEnum, PlayerStateBase> _playerStates;
    
    private int _playerScore = 0;
    private int _opponentScore = 0;
    
    [Header("Player Scriptable Objects")]
    [SerializeField] private SoUniversalInputEvents soUniversalInputEvents;
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private SoTimerEvents soTimerEvents;
    [SerializeField] private SoContainerEvents soContainerEvents;
    
    [Space]
    [Header("Player Gameplay Scripts")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CardManager cardManager;
    [SerializeField] private CardHighlightController cardHighlightController;
    [SerializeField] private CardPickController cardPickController;
    [SerializeField] private CardSelectionController cardSelectionController;
    [SerializeField] private CameraController cameraController;
    
    [Space]
    [Header("Player Menu Scripts")]
    [SerializeField] private InputUIManager inputUIManager;
    [SerializeField] private SoUIEvents soUIEvents;

    [Space] 
    [Header("Enemy Scriptable Objects")] 
    [SerializeField] private SoCardEvents enemySoCardEvents;
    [SerializeField] private BehaviourTreeOwner enemyBehaviourTreeOwner;
    
    #endregion

    private void Awake()
    {
        _gameStateMachine = new StateMachine<GameStateEnum>();
        _playerStateMachine = new StateMachine<PlayerStateEnum>();

        _gameStateMachine.OnStateChanged += HandleGameStateChanged;
        _playerStateMachine.OnStateChanged += HandlePlayerStateChanged;

        soGameStateEvents.OnGameStateChange += HandleSoGameStateChange;
        soGameStateEvents.OnPlayerStateChange += HandlePlayerStateChange;
        soGameStateEvents.OnRevertGameState += HandleRevertSoGameState;
        soGameStateEvents.OnRevertPlayerState += HandleRevertPlayerState;
        
        soGameStateEvents.OnEndRound += HandleEndRound;
        soGameStateEvents.OnPlayerWonRound += HandleRoundWonByPlayer;

        List<GameStateBase> gameStateInstances = new List<GameStateBase>
        {
            new GameStates.SetupState(_gameStateMachine, inputManager, soGameStateEvents),
            new GameStates.BeginRoundState(_gameStateMachine, soGameStateEvents, soCardEvents, enemySoCardEvents,
                soAnimationEvents),
            new GameStates.PlayerTurnState(_gameStateMachine, soGameStateEvents, soAnimationEvents, soTimerEvents,
                soContainerEvents, cardPickController, soUniversalInputEvents),
            new GameStates.OpponentTurnState(_gameStateMachine, soGameStateEvents, soAnimationEvents, soTimerEvents,
                soContainerEvents, enemyBehaviourTreeOwner, soCardEvents, soUniversalInputEvents),
            new GameStates.SkipPlayerTurnState(_gameStateMachine, soGameStateEvents),
            new GameStates.SkipOpponentTurnState(_gameStateMachine, soGameStateEvents, soCardEvents,
                soUniversalInputEvents),
            new GameStates.EndRoundState(_gameStateMachine, soGameStateEvents, soCardEvents, enemySoCardEvents,
                soContainerEvents)
        };

        _gameStates = gameStateInstances.ToDictionary(state => state.StateType, state => state);

        List<PlayerStateBase> playerStates = new List<PlayerStateBase>()
        {
            new PlayerStates.TurnIdleState(_playerStateMachine, cardHighlightController, cardPickController),
            new PlayerStates.PickedCardState(_playerStateMachine, cardHighlightController),
            new PlayerStates.PlacedCardMergerState(_playerStateMachine),
            new PlayerStates.PlacedCardTableState(_playerStateMachine),
            new PlayerStates.AllPlacedCardsState(_playerStateMachine, cardHighlightController),
            new PlayerStates.OpponentTurnIdleState(_playerStateMachine, soCardEvents, cardPickController),
            new PlayerStates.BeginRoundState(_playerStateMachine, cardSelectionController),
            new PlayerStates.EndRoundState(_playerStateMachine, cardSelectionController),
            new PlayerStates.LookAroundState(_playerStateMachine, cardSelectionController),
            new PlayerStates.PauseState(_playerStateMachine, inputManager, inputUIManager, soUniversalInputEvents,
                soCardEvents, soGameStateEvents)
        };
        
        _playerStates = playerStates.ToDictionary(state => state.StateType, state => state);
    }

    private void OnDestroy()
    {
        _gameStateMachine.OnStateChanged -= HandleGameStateChanged;
        _playerStateMachine.OnStateChanged -= HandlePlayerStateChanged;
        
        soGameStateEvents.OnGameStateChange -= HandleSoGameStateChange;
        soGameStateEvents.OnPlayerStateChange -= HandlePlayerStateChange;
        soGameStateEvents.OnRevertGameState -= HandleRevertSoGameState;
        soGameStateEvents.OnRevertPlayerState -= HandleRevertPlayerState;
        
        soGameStateEvents.OnEndRound -= HandleEndRound;
        soGameStateEvents.OnPlayerWonRound -= HandleRoundWonByPlayer;
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
        if (soGameStateEvents.playerHasEndedRound && soGameStateEvents.opponentHasEndedRound)
        {
            newState = GameStateEnum.EndRound;
        }
        
        switch (newState)
        {
            case GameStateEnum.PlayerTurn when soGameStateEvents.playerHasEndedRound:
                newState = GameStateEnum.SkipPlayerTurn;
                break;
            case GameStateEnum.OpponentTurn when soGameStateEvents.opponentHasEndedRound:
                newState = GameStateEnum.SkipOpponentTurn;
                break;
        }

        if (_gameStates.TryGetValue(newState, out var gameState))
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
        if (_playerStates.TryGetValue(newState, out var state))
        {
            _playerStateMachine.ChangeState(state);
        }
        else
        {
            Debug.LogWarning($"Player state {newState} not found.");
        }
    }

    private void HandleRevertSoGameState(out bool success) => success = _gameStateMachine.RevertToPreviousState();

    private void HandleRevertPlayerState(out bool success) => success = _playerStateMachine.RevertToPreviousState();
    
    private void HandleGameStateChanged(StateBase<GameStateEnum> newState)
    {
        soGameStateEvents.UpdateCurrentGameState(newState.StateType);
    }

    private void HandlePlayerStateChanged(StateBase<PlayerStateEnum> newState)
    {
        soGameStateEvents.UpdateCurrentPlayerState(newState.StateType);
    }

    private void HandleEndRound(OwnerType owner)
    {
        switch (owner)
        {
            case OwnerType.Player:
                soGameStateEvents.playerHasEndedRound = true;
                break;
            case OwnerType.Enemy:
                soGameStateEvents.opponentHasEndedRound = true;
                break;
        }
    }

    private void HandleRoundWonByPlayer(bool wonByPlayer)
    {
        if (wonByPlayer)
            _playerScore++;
        else
            _opponentScore++;
        
        soGameStateEvents.playerHasEndedRound = false;
        soGameStateEvents.opponentHasEndedRound = false;
        
        soUIEvents.RaiseSetCrystalsAmount(_playerScore, _opponentScore);
        
        if (_playerScore == 2 || _opponentScore == 2)
        {
            inputManager.enabled = false;
            soUIEvents.RaiseEndGame(wonByPlayer);
        }
    }
}
