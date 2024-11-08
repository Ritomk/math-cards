using System.Collections;

namespace PlayerStates
{
    public class PauseState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.PauseGame;

        private InputManager _inputManager;
        private InputUIManager _inputUIManager;
        private SoUniversalInputEvents _soUniversalInputEvents;
        private SoCardEvents _soCardEvents;
        private SoGameStateEvents _soGameStateEvents;

        public PauseState(StateMachine<PlayerStateEnum> stateMachine, InputManager inputManager, InputUIManager inputUIManager, SoUniversalInputEvents soUniversalInputEvents, SoCardEvents soCardEvents, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _inputManager = inputManager;
            _inputUIManager = inputUIManager;
            _soUniversalInputEvents = soUniversalInputEvents;
            _soCardEvents = soCardEvents;
            _soGameStateEvents = soGameStateEvents;
        }

        public override IEnumerator Enter()
        {
            _inputManager.enabled = false;
            _inputUIManager.enabled = true;
            
            _soGameStateEvents.RaisePauseGame(true);
            CoroutineHelper.PauseAll(); 
            
            yield return null;
        }

        public override IEnumerator Exit()
        {
            _inputManager.enabled = true;
            _inputUIManager.enabled = false;
            
            _soGameStateEvents.RaisePauseGame(false);
            CoroutineHelper.ResumeAll();
            
            _soCardEvents.RaiseCardSelectionReset();
            _soUniversalInputEvents.RaiseMouseMove();
            
            yield return null;
        }
    }
}