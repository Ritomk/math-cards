using System.Collections;
using UnityEngine;

namespace GameStates
{
    public class SkipOpponentTurnState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.SkipOpponentTurn;
        
        private SoGameStateEvents _soGameStateEvents;
        private SoCardEvents _soCardEvents;
        private SoUniversalInputEvents _soUniversalInputEvents;

        public SkipOpponentTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoCardEvents soCardEvents, SoUniversalInputEvents soUniversalInputEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
            _soUniversalInputEvents = soUniversalInputEvents;
        }

        public override IEnumerator Enter()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
            
            yield return null;
        }

        public override IEnumerator Exit()
        {
            _soCardEvents.RaiseCardSelectionReset();
            _soUniversalInputEvents.RaiseMouseMove();
            
            yield return null;
        }
    }   
}