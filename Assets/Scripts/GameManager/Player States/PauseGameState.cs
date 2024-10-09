namespace PlayerStates
{
    public class PauseGameState : GameStateBase
    {
        private InputManager _inputManager;
        private InputUIManager _inputUIManager;
        private SoUniversalInputEvents _soUniversalInputEvents;
        private SoCardEvents _soCardEvents;


        public PauseGameState(GameStateMachine stateMachine, InputManager inputManager, InputUIManager inputUIManager, SoUniversalInputEvents soUniversalInputEvents, SoCardEvents soCardEvents) : base(stateMachine)
        {
            _inputManager = inputManager;
            _inputUIManager = inputUIManager;
            _soUniversalInputEvents = soUniversalInputEvents;
            _soCardEvents = soCardEvents;
        }

        public override void Enter()
        {
            _inputManager.enabled = false;
            _inputUIManager.enabled = true;
            CoroutineHelper.Pause();
        }

        public override void Exit()
        {
            _inputManager.enabled = true;
            _inputUIManager.enabled = false;
            CoroutineHelper.Resume();
            _soCardEvents.RaiseCardSelectionReset();
            _soUniversalInputEvents.RaiseMouseMove();
        }
    }
}