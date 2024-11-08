using System.Collections;
using UnityEngine;

namespace PlayerStates
{
    public class OpponentTurnIdleState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.OpponentTurnIdle;

        private SoCardEvents _soCardEvents;
        private CardPickController _cardPickController;

        public OpponentTurnIdleState(StateMachine<PlayerStateEnum> stateMachine, SoCardEvents soCardEvents, CardPickController cardPickController) : base(stateMachine)
        {
            _soCardEvents = soCardEvents;
            _cardPickController = cardPickController;
        }

        public override IEnumerator Enter()
        {
            _cardPickController.enabled = false;
            
            yield return null;
        }
    }
}