using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    [SerializeField] private SoGameStateEvents gameStateEvents;

    private PlayerInput _playerInput;
    private InputActionMap _inputActionMap;

    private InputAction _mouseMovement;
    private InputAction _leftClick;
    private InputAction _rightClick;
    private InputAction _endTurn;
    private InputAction _escapePress;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _inputActionMap = _playerInput.actions.FindActionMap("Gameplay");
        
        _mouseMovement = _inputActionMap.FindAction("MouseMove");
        _leftClick = _inputActionMap.FindAction("LeftClick");
        _rightClick = _inputActionMap.FindAction("RightClick");
        _endTurn = _inputActionMap.FindAction("EndTurn");
        _escapePress = _inputActionMap.FindAction("EscapePress");
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
        _endTurn.performed += HandleEndTurn;
        _escapePress.performed += HandleEscapePress;

        //Spaghetti Code
        inputEvents.RaiseCameraReset(_rightClick.ReadValue<float>() < 0.5f);
    }

    private void OnDisable()
    {
        _mouseMovement.performed -= HandleMouseMove;
        _leftClick.performed -= HandleMouseLeftClick;
        _leftClick.canceled -= HandleMouseLeftClick;
        _rightClick.performed -= HandleMouseRightClick;
        _endTurn.performed -= HandleEndTurn;
        _escapePress.performed -= HandleEscapePress;
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
                gameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.LookAround);
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
    
    private void HandleEndTurn(InputAction.CallbackContext obj)
    {
        if (gameStateEvents.CurrentPlayerState == PlayerStateEnum.AllCardsPlaced)
        {
            gameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
        }
        else if (gameStateEvents.CurrentGameState == GameStateEnum.OpponentTurn)
        {
            gameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        }
    }

    private void HandleEscapePress(InputAction.CallbackContext context)
    {
        if (gameStateEvents.CurrentPlayerState is PlayerStateEnum.CardPicked
            or PlayerStateEnum.LookAround)
        {
            gameStateEvents.RaiseOnRevertPlayerState();
        }
        
        gameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PauseGame);
    }
}
