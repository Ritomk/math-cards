using System.Collections;
using UnityEngine;

namespace GameStates
{
    public class SkipPlayerTurnState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.SkipPlayerTurn;
        
        private SoGameStateEvents _soGameStateEvents;
        
        public SkipPlayerTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
        }
        
        public override IEnumerator Enter()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.OpponentTurn);
            
            yield return null;
        }
    }
}
