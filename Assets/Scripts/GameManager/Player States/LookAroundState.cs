namespace PlayerStates
{
    public class LookAroundState : GameStateBase
    {
        private CardSelectionController _cardSelectionController;


        public LookAroundState(GameStateMachine stateMachine, CardSelectionController cardSelectionController) : base(stateMachine)
        {
            _cardSelectionController = cardSelectionController;
        }

        public override void Enter()
        {
            _cardSelectionController.enabled = false;
        }

        public override void Exit()
        {
            _cardSelectionController.enabled = true;
        }
    }
}