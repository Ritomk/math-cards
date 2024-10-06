using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class OpponentTurnState : GameStateBase
    {
        private SoGameStateEvents _soGameStateEvents;

        public OpponentTurnState(GameStateMachine stateMachine, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
        }

        public override void Enter()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        }
    }   
}
