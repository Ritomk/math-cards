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

        public override void Enter()
        {
            _cardHighlightController.enabled = false;
        }

        public override void Exit()
        {
            _cardHighlightController.enabled = true;
        }
    }
}

