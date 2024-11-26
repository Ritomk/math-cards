using UnityEngine;


namespace PlayerStates
{
    public class PlacedCardMergerState : PlacedCardStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.CardPlacedMerger;

        public PlacedCardMergerState(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine)
        { }
    }
}