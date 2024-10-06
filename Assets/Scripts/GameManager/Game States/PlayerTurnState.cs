using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class PlayerTurnState : GameStateBase
    {
        private SoAnimationEvents _soAnimationEvents;
        private SoGameStateEvents _soGameStateEvents;
        private SoTimerEvents _soTimerEvents;
        private CardPickController _cardPickController;


        public PlayerTurnState(GameStateMachine stateMachine, SoAnimationEvents soAnimationEvents, SoGameStateEvents soGameStateEvents, SoTimerEvents soTimerEvents, CardPickController cardPickController) : base(stateMachine)
        {
            _soAnimationEvents = soAnimationEvents;
            _soGameStateEvents = soGameStateEvents;
            _soTimerEvents = soTimerEvents;
            _cardPickController = cardPickController;
        }

        public override void Enter()
        {
            _soTimerEvents.OnTimerComplete += TurnEnded;
            
            _soTimerEvents.RaiseStartTimer(15f);
            
            _cardPickController.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
        }

        public override void Exit()
        {
            _soTimerEvents.OnTimerComplete -= TurnEnded;
            
            _cardPickController.enabled = false;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.OpponentTurnIdle);
            _soAnimationEvents.RaiseCoinFlipAnimation();
        }

        private void TurnEnded()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
        }
    }
}

