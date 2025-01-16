using System.Collections;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace GameStates
{
    public class OpponentTurnState : TurnStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.OpponentTurn;

        private BehaviourTreeOwner _behaviourTreeOwner;
        private SoCardEvents _soCardEvents;
        private SoUniversalInputEvents _soUniversalInputEvents;

        public OpponentTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoAnimationEvents soAnimationEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents,
            BehaviourTreeOwner behaviourTreeOwner, SoCardEvents soCardEvents,
            SoUniversalInputEvents soUniversalInputEvents) : base(stateMachine, soGameStateEvents, soAnimationEvents,
            soTimerEvents, soContainerEvents)
        {
            _behaviourTreeOwner = behaviourTreeOwner;
            _soCardEvents = soCardEvents;
            _soUniversalInputEvents = soUniversalInputEvents;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();
            
            _soAnimationEvents.RaiseToggleChestAnimation(OwnerType.Enemy,true);
            
            _behaviourTreeOwner.enabled = true;
            _soGameStateEvents.RaisePlayerStateChange(PlayerStateEnum.OpponentTurnIdle);
            
            _soCardEvents.RaiseCardSelectionReset();
            _soUniversalInputEvents.RaiseMouseMove();

            yield return null;
        }

        public override IEnumerator Exit()
        {
            _behaviourTreeOwner.enabled = false;
            return base.Exit();
        }

        protected override void TurnEnded()
        {
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        }
    }   
}
