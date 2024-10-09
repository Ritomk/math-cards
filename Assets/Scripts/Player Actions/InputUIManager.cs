using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputUIManager : MonoBehaviour
{
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    
    private PlayerInput _playerInput;
    private InputActionMap _inputActionMap;
    
    private InputAction _spacePress;
    private InputAction _escapePress;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _inputActionMap = _playerInput.actions.FindActionMap("Menu");
        
        _spacePress = _inputActionMap.FindAction("SpacePress");
        _escapePress = _inputActionMap.FindAction("EscapePress");
    }

    private void OnEnable()
    {
        _playerInput.SwitchCurrentActionMap("Menu");
        
        _spacePress.performed += HandleSpacePress;
        _escapePress.performed += HandleEscapePress;
    }

    private void OnDisable()
    {
        _spacePress.performed -= HandleSpacePress;
        _escapePress.performed -= HandleEscapePress;
    }

    private void HandleSpacePress(InputAction.CallbackContext obj)
    {
        
    }
    
    private void HandleEscapePress(InputAction.CallbackContext obj)
    {
        Debug.Log("Escape");
        soGameStateEvents.RaiseOnRevertPlayerState();
    }
}
