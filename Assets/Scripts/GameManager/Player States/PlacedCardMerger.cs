using UnityEngine;


namespace PlayerStates
{
    public class PlacedCardMerger : PlayerStateBase
    {
        public PlacedCardMerger(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine)
        {
        }

        public override PlayerStateEnum StateType { get; }
    }
}