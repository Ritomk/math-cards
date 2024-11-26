using UnityEngine;


namespace PlayerStates
{
    public abstract class PlacedCardStateBase : PlayerStateBase
    {
        protected PlacedCardStateBase(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine)
        {
        }
    }
}