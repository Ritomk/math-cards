using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStates
{
    public class AllPlacedCardsState : PlayerStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.CardsPlaced;

        private CardHighlightController _cardHighlightController;


        public AllPlacedCardsState(StateMachine<PlayerStateEnum> stateMachine, CardHighlightController cardHighlightController) : base(stateMachine)
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
