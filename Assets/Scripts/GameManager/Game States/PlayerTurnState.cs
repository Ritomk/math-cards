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
        private SoContainerEvents _soContainerEvents;
        private SoCardEvents _soCardEvents;
        private CardPickController _cardPickController;


        public PlayerTurnState(GameStateMachine stateMachine, SoAnimationEvents soAnimationEvents, SoGameStateEvents soGameStateEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents, SoCardEvents soCardEvents, CardPickController cardPickController) : base(stateMachine)
        {
            _soAnimationEvents = soAnimationEvents;
            _soGameStateEvents = soGameStateEvents;
            _soTimerEvents = soTimerEvents;
            _soContainerEvents = soContainerEvents;
            _soCardEvents = soCardEvents;
            _cardPickController = cardPickController;
        }

        public override void Enter()
        {
            _soTimerEvents.OnTimerComplete += TurnEnded;
            
            _soTimerEvents.RaiseStartTimer(30f);
            
            _soAnimationEvents.RaiseToggleChestAnimation(true);
            
            _cardPickController.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
        }

        public override void Exit()
        {
            _soTimerEvents.OnTimerComplete -= TurnEnded;
            
            _soCardEvents.RaiseCardSelectionReset();
            _soCardEvents.RaiseCardSelected(null);
 
            _soContainerEvents.RaiseChangeCardsState(CardData.CardState.NonPickable);
            
            _soAnimationEvents.RaiseToggleChestAnimation(false);
            
            _soContainerEvents.RaiseValidateCardPlacement();
            _soContainerEvents.RaiseMergeCards();
            
            _soAnimationEvents.RaiseCoinFlipAnimation();
            _soTimerEvents.RaiseStopTimer();
        }

        private void TurnEnded()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
        }
    }
}

