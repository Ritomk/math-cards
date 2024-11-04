﻿using System.Collections;

namespace PlayerStates
{
    public class PlayerTurnIdleState : GameStateBase
    {
        private CardHighlightController _cardHighlightController;
        private CardPickController _cardPickController;


        public PlayerTurnIdleState(GameStateMachine stateMachine, CardHighlightController cardHighlightController, CardPickController cardPickController) : base(stateMachine)
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