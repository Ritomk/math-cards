using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerPickedCardState : GameStateBase
    {
        private CardHighlightController _cardHighlightController;


        public PlayerPickedCardState(GameStateMachine stateMachine, CardHighlightController cardHighlightController) : base(stateMachine)
        {
            _cardHighlightController = cardHighlightController;
        }

        public override IEnumerator Enter()
        {
            _cardHighlightController.enabled = false;
            
            yield return null;
        }

        public override IEnumerator Exit()
        {
            _cardHighlightController.enabled = true;
            
            yield return null;
        }
    }
}

