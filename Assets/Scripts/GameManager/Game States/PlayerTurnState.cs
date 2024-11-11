using System.Collections;

namespace GameStates
{
    public class PlayerTurnState : TurnStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.PlayerTurn;
        
        private SoGameStateEvents _soGameStateEvents;
        private CardPickController _cardPickController;
        private SoUniversalInputEvents _soUniversalInputEvents;

        public PlayerTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoAnimationEvents soAnimationEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents,
            CardPickController cardPickController, SoUniversalInputEvents soUniversalInputEvents)
            : base(stateMachine, soGameStateEvents, soAnimationEvents, soTimerEvents, soContainerEvents)
        {
            _soGameStateEvents = soGameStateEvents;
            _cardPickController = cardPickController;
            _soUniversalInputEvents = soUniversalInputEvents;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            _cardPickController.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
            _soUniversalInputEvents.RaiseMouseMove();
            
            yield return null;
        }

        public override IEnumerator Exit()
        {
            _soUniversalInputEvents.RaiseCardPick(false);
            _soUniversalInputEvents.RaiseMouseMove();
            _soUniversalInputEvents.RaiseCameraReset(true);
            
            yield return base.Exit();
        }

        protected override void TurnEnded()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
        }
    }
}

