using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerPlacedCardState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.CardPlaced;

        private CardHighlightController _cardHighlightController;


        public PlayerPlacedCardState(StateMachine<PlayerStateEnum> stateMachine, CardHighlightController cardHighlightController) : base(stateMachine)
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
