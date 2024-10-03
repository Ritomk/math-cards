using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class SetupState : GameStateBase
    {
        private InputManager _inputManager;


        public SetupState(GameStateMachine stateMachine, InputManager inputManager) : base(stateMachine)
        {
            _inputManager = inputManager;
        }

        public override void Enter()
        {
        
        }

        public override void Exit()
        {
            _inputManager.enabled = true;
        }
    }
}
