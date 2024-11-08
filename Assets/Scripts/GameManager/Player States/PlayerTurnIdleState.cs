using System.Collections;

namespace PlayerStates
{
    public class PlayerTurnIdleState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.PlayerTurnIdle;

        private CardHighlightController _cardHighlightController;
        private CardPickController _cardPickController;


        public PlayerTurnIdleState(StateMachine<PlayerStateEnum> stateMachine, CardHighlightController cardHighlightController, CardPickController cardPickController) : base(stateMachine)
        {
            _cardHighlightController = cardHighlightController;
            _cardPickController = cardPickController;
        }

        public override IEnumerator Enter()
        {
            _cardHighlightController.enabled = true;
            _cardPickController.enabled = true;
            
            yield return null;
        }
    }
}