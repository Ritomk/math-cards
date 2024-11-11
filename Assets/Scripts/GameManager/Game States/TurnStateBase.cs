using System.Collections;

namespace GameStates
{
    public abstract class TurnStateBase : GameStateBase
    {
        protected SoGameStateEvents _soGameStateEvents;
        protected SoAnimationEvents _soAnimationEvents;
        protected SoTimerEvents _soTimerEvents;
        protected SoContainerEvents _soContainerEvents;


        protected TurnStateBase(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoAnimationEvents soAnimationEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents)
            : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soAnimationEvents = soAnimationEvents;
            _soTimerEvents = soTimerEvents;
            _soContainerEvents = soContainerEvents;
        }

        public override IEnumerator Enter()
        {
            _soTimerEvents.OnTimerComplete += TurnEnded;
            
            _soTimerEvents.RaiseStartTimer(GetTurnDuration());
            
            _soAnimationEvents.RaiseToggleChestAnimation(true);
            
            yield return null;
        }
        
        public override IEnumerator Exit()
        {
            _soTimerEvents.OnTimerComplete -= TurnEnded;
            
            _soContainerEvents.RaiseChangeCardsState(CardData.CardState.NonPickable);
            
            _soContainerEvents.RaiseValidateCardPlacement();
            _soContainerEvents.RaiseMergeCards();
            
            _soAnimationEvents.RaiseCoinFlipAnimation();
            _soTimerEvents.RaiseStopTimer();
            
            CoroutineHelper.DebugRunningCoroutines(CoroutineHelper.DebugType.Both);

            yield return CoroutineHelper.WaitForAllCoroutines(5f);
        }

        protected abstract void TurnEnded();

        protected virtual float GetTurnDuration() => 5f;
    }
}