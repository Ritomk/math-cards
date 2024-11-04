using System.Collections;

namespace PlayerStates
{
    public class LookAroundState : GameStateBase
    {
        private CardSelectionController _cardSelectionController;


        public LookAroundState(GameStateMachine stateMachine, CardSelectionController cardSelectionController) : base(stateMachine)
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