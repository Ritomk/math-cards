using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerPlacedCardState : GameStateBase
    {
        private CardHighlightController _cardHighlightController;


        public PlayerPlacedCardState(GameStateMachine stateMachine, CardHighlightController cardHighlightController) : base(stateMachine)
        {
            _cardHighlightController = cardHighlightController;
        }

        public override IEnumerator Enter()
        {
            _cardHighlightController.enabled = true;
            
            yield return null;
        }
    }   
}
