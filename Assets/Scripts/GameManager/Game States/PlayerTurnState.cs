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
        private SoUniversalInputEvents _soUniversalInputEvents;


        public PlayerTurnState(GameStateMachine stateMachine, SoAnimationEvents soAnimationEvents, SoGameStateEvents soGameStateEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents, SoCardEvents soCardEvents, CardPickController cardPickController, SoUniversalInputEvents soUniversalInputEvents) : base(stateMachine)
        {
            _soAnimationEvents = soAnimationEvents;
            _soGameStateEvents = soGameStateEvents;
            _soTimerEvents = soTimerEvents;
            _soContainerEvents = soContainerEvents;
            _soCardEvents = soCardEvents;
            _cardPickController = cardPickController;
            _soUniversalInputEvents = soUniversalInputEvents;
        }

        public override IEnumerator Enter()
        {
            _soTimerEvents.OnTimerComplete += TurnEnded;
            
            _soTimerEvents.RaiseStartTimer(5f);
            
            _soAnimationEvents.RaiseToggleChestAnimation(true);
            
            _cardPickController.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
            _soUniversalInputEvents.RaiseMouseMove();
            
            yield return null;
        }

        public override IEnumerator Exit()
        {
            Debug.Log($"Start Exiting Player State | Running Coroutines {CoroutineHelper.GetRunningCoroutinesCount()}");
            _soTimerEvents.OnTimerComplete -= TurnEnded;
            
            _soUniversalInputEvents.RaiseCardPick(false);
            _soUniversalInputEvents.RaiseMouseMove();
            _soUniversalInputEvents.RaiseCameraReset(true);
            
            _soContainerEvents.RaiseChangeCardsState(CardData.CardState.NonPickable);
            
            _soContainerEvents.RaiseValidateCardPlacement();
            _soContainerEvents.RaiseMergeCards();
            
            _soAnimationEvents.RaiseCoinFlipAnimation();
            _soTimerEvents.RaiseStopTimer();
            
            Debug.Log($"End Exiting Player State | Running Coroutines {CoroutineHelper.GetRunningCoroutinesCount()}");
            CoroutineHelper.DebugRunningCoroutines(CoroutineHelper.DebugType.Both);

            yield return CoroutineHelper.WaitForAllCoroutines(5f);
        }

        private void TurnEnded()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
        }
    }
}

