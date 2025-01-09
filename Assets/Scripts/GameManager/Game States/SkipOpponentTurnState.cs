using System.Collections;
using UnityEngine;

namespace GameStates
{
    public class SkipOpponentTurnState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.SkipOpponentTurn;
        
        private SoGameStateEvents _soGameStateEvents;
     
        public SkipOpponentTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
        }
        
        public override IEnumerator Enter()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
            
            yield return null;
        }
    }   
}