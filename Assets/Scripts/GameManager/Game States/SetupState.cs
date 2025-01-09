using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class SetupState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.Setup;
        
        private InputManager _inputManager;
        private SoGameStateEvents _soGameStateEvents;

        public SetupState(StateMachine<GameStateEnum> stateMachine, InputManager inputManager, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _inputManager = inputManager;
            _soGameStateEvents = soGameStateEvents;
        }

        public override IEnumerator Enter()
        {
            yield return new WaitForSeconds(0.1f);
            
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.BeginRound);
        }

        public override IEnumerator Exit()
        {
            _inputManager.enabled = true;
            
            yield return null;
        }
    }
}
