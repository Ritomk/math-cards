using UnityEngine;


namespace PlayerStates
{
    public class PlacedCardTableState : PlacedCardStateBase
    {
        public override PlayerStateEnum StateType => PlayerStateEnum.CardPlacedTable;

        public PlacedCardTableState(StateMachine<PlayerStateEnum> stateMachine) : base(stateMachine)
        { }
    }
}