using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class OpponentTurnState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.OpponentTurn;

        private SoGameStateEvents _soGameStateEvents;

        public OpponentTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
        }

        public override IEnumerator Enter()
        {
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.OpponentTurnIdle);

            yield return null;
        }
    }   
}
