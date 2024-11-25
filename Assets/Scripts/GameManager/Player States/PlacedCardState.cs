using UnityEngine;


namespace PlayerStates
{
    public class PlacedCardState : PlayerStateBase
    {
        public override PlayerStateEnum StateType { get; }
        
        public PlacedCardState(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine)
        { }
    }
}