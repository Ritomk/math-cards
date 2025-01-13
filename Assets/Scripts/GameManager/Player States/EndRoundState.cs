using System.Collections;

namespace PlayerStates
{
    public class EndRoundState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.EndRound;
        
        private CardSelectionController _cardSelectionController;

        public EndRoundState(StateMachine<PlayerStateEnum> stateMachine,
            CardSelectionController cardSelectionController) : base(stateMachine)
        {
            _cardSelectionController = cardSelectionController;
        }

        public override IEnumerator Enter()
        {
            _cardSelectionController.enabled = false;

            yield return null;
        }

        public override IEnumerator Exit()
        {
            _cardSelectionController.enabled = true;

            yield return null;
        }
    }
}