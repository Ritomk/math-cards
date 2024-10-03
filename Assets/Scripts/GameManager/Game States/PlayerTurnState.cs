using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class PlayerTurnState : GameStateBase
    {
        private CardPickController _cardPickController;

        public PlayerTurnState(GameStateMachine stateMachine, CardPickController cardPickController) : base(stateMachine)
        {
            _cardPickController = cardPickController;
        }

        public override void Enter()
        {
            _cardPickController.enabled = true;
        }
    }
}

