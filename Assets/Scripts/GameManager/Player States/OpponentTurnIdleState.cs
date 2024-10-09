using UnityEngine;

namespace PlayerStates
{
    public class OpponentTurnIdleState : GameStateBase
    {
        private SoCardEvents _soCardEvents;
        private CardPickController _cardPickController;

        public OpponentTurnIdleState(GameStateMachine stateMachine, SoCardEvents soCardEvents, CardPickController cardPickController) : base(stateMachine)
        {
            _soCardEvents = soCardEvents;
            _cardPickController = cardPickController;
        }

        public override void Enter()
        {
            _soCardEvents.RaiseCardSelectionReset();
            _soCardEvents.RaiseCardSelected(null);
            _cardPickController.enabled = false;
        }
    }
}