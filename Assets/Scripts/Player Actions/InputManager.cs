using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    [SerializeField] private SoGameStateEvents gameStateEvents;
    [SerializeField] private SoUIEvents uiEvents;

    private PlayerInput _playerInput;
    private InputActionMap _inputActionMap;

    private InputAction _mouseMovement;
    private InputAction _leftClick;
    private InputAction _rightClick;
    private InputAction _escapePress;
    private InputAction _endTurn;
    private InputAction _endRound;
    private InputAction _enableSlider;
    
    private float _spacePressedTime;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputActionMap = _playerInput.actions.FindActionMap("Gameplay");
        
        _mouseMovement = _inputActionMap.FindAction("MouseMove");
        _leftClick = _inputActionMap.FindAction("LeftClick");
        _rightClick = _inputActionMap.FindAction("RightClick");
        _escapePress = _inputActionMap.FindAction("EscapePress");
        
        _endTurn = _inputActionMap.FindAction("EndTurn");
        _endRound = _inputActionMap.FindAction("EndRound");
        _enableSlider = _inputActionMap.FindAction("EnableSlider");
    }

    private void Start()
    {
        Application.targetFrameRate = -1;
    }

    private void OnEnable()
    {
        _playerInput.SwitchCurrentActionMap("Gameplay");
        
        _mouseMovement.performed += HandleMouseMove;
        _leftClick.performed += HandleMouseLeftClick;
        _leftClick.canceled += HandleMouseLeftClick;
        _rightClick.performed += HandleMouseRightClick;
        _escapePress.performed += HandleEscapePress;
        
        _endTurn.performed += HandleEndTurn;
        _endTurn.canceled += HandleEndTurn;
        _endRound.performed += HandleEndRound;
        _enableSlider.performed += HandleEnableSlider;
        _enableSlider.canceled += HandleEnableSlider;


        //Spaghetti Code
        inputEvents.RaiseCameraReset(_rightClick.ReadValue<float>() < 0.5f);
    }

    private void OnDisable()
    {
        _mouseMovement.performed -= HandleMouseMove;
        _leftClick.performed -= HandleMouseLeftClick;
        _leftClick.canceled -= HandleMouseLeftClick;
        _rightClick.performed -= HandleMouseRightClick;
        _escapePress.performed -= HandleEscapePress;
        
        _endTurn.performed -= HandleEndTurn;
        _endTurn.canceled -= HandleEndTurn;
        _endRound.performed -= HandleEndRound;
        _enableSlider.performed -= HandleEnableSlider;
        _enableSlider.canceled -= HandleEnableSlider;
    }

    private void HandleMouseMove(InputAction.CallbackContext context)
    {
        if(_rightClick.IsPressed())
        {
            inputEvents.RaiseLookAround(context.ReadValue<Vector2>());
            
            if (gameStateEvents.CurrentPlayerState != PlayerStateEnum.LookAround)
            {
                if (gameStateEvents.CurrentPlayerState == PlayerStateEnum.CardPicked)
                {
                    gameStateEvents.RaiseOnRevertPlayerState();
                }
                gameStateEvents.RaisePlayerStateChange(PlayerStateEnum.LookAround);
            }
        }
        else
        {
            inputEvents.RaiseMouseMove();
            
            if (gameStateEvents.CurrentPlayerState != PlayerStateEnum.LookAround) return;

            gameStateEvents.RaiseOnRevertPlayerState();
        }
    }

    private void HandleMouseLeftClick(InputAction.CallbackContext context)
    {
        if(context.interaction is HoldInteraction)
        {
            if (context.performed)
            {
                inputEvents.RaiseCardPick(true);
                inputEvents.RaiseMouseMove();
            }
            else if (context.canceled)
            {
                inputEvents.RaiseCardPick(false);
                inputEvents.RaiseMouseMove();
            }
        }
    }

    private void HandleMouseRightClick(InputAction.CallbackContext context) => inputEvents.RaiseCameraReset(!context.control.IsPressed());
    
    private void HandleEscapePress(InputAction.CallbackContext context)
    {
        if (gameStateEvents.CurrentPlayerState is PlayerStateEnum.CardPicked
            or PlayerStateEnum.LookAround)
        {
            gameStateEvents.RaiseOnRevertPlayerState();
        }
        
        gameStateEvents.RaisePlayerStateChange(PlayerStateEnum.PauseGame);
    }
    
    private void HandleEndTurn(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            _spacePressedTime = Time.timeSinceLevelLoad;
        }
        else if (context.canceled)
        {
            var spaceHoldTime = Time.timeSinceLevelLoad - _spacePressedTime;
            
            if (spaceHoldTime < 0.5f)
            {
                if (gameStateEvents.CurrentPlayerState == PlayerStateEnum.EndRound)
                {
                    gameStateEvents.RaiseGameStateChange(GameStateEnum.BeginRound);
                }
                else if (gameStateEvents.CurrentPlayerState is PlayerStateEnum.AllCardsPlaced
                    or PlayerStateEnum.CardPlacedTable)
                {
                    gameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
                }
            }
        }
    }

    private void HandleEndRound(InputAction.CallbackContext context)
    {
        if (context.interaction is not HoldInteraction) return;

        if (gameStateEvents.CurrentPlayerState is PlayerStateEnum.AllCardsPlaced
            or PlayerStateEnum.CardPlacedTable or PlayerStateEnum.PlayerTurnIdle)
        {
            gameStateEvents.RaiseEndRound(OwnerType.Player);
            gameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
            uiEvents.RaiseEndSlider();
        }
    }

    private void HandleEnableSlider(InputAction.CallbackContext context)
    {
        if (context.interaction is not HoldInteraction) return;

        if (gameStateEvents.CurrentPlayerState is PlayerStateEnum.AllCardsPlaced
            or PlayerStateEnum.CardPlacedTable or PlayerStateEnum.PlayerTurnIdle)
        {
            if (context.performed)
            {
                uiEvents.RaiseStartSlider();
            }
            else if (context.canceled)
            {
                uiEvents.RaiseEndSlider();
            }
        }
    }
}
