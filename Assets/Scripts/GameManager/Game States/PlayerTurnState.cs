using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class PlayerTurnState : GameStateBase
    {
        private SoAnimationEvents _soAnimationEvents;
        private SoGameStateEvents _soGameStateEvents;
        private CardPickController _cardPickController;


        public PlayerTurnState(GameStateMachine stateMachine, SoAnimationEvents soAnimationEvents, SoGameStateEvents soGameStateEvents, CardPickController cardPickController) : base(stateMachine)
        {
            _soAnimationEvents = soAnimationEvents;
            _soGameStateEvents = soGameStateEvents;
            _cardPickController = cardPickController;
        }

        public override void Enter()
        {
            _cardPickController.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
        }

        public override void Exit()
        {
            _cardPickController.enabled = false;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.OpponentTurnIdle);
            _soAnimationEvents.RaiseCoinFlipAnimation();
        }
    }
}

