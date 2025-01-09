using System.Collections;
using NodeCanvas.BehaviourTrees;
using UnityEngine;

namespace GameStates
{
    public class OpponentTurnState : TurnStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.OpponentTurn;

        private BehaviourTreeOwner _behaviourTreeOwner;
        
        public OpponentTurnState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoAnimationEvents soAnimationEvents, SoTimerEvents soTimerEvents, SoContainerEvents soContainerEvents,
            BehaviourTreeOwner behaviourTreeOwner) 
            : base(stateMachine, soGameStateEvents, soAnimationEvents, soTimerEvents, soContainerEvents)
        {
            _behaviourTreeOwner = behaviourTreeOwner;
        }
        
        public override IEnumerator Enter()
        {
            yield return base.Enter();

            _soAnimationEvents.RaiseToggleChestAnimation(OwnerType.Enemy,true);
            
            _behaviourTreeOwner.enabled = true;
            _soGameStateEvents.RaisePlayerStateChange(PlayerStateEnum.OpponentTurnIdle);

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

        protected override float GetTurnDuration() => 10f;
    }   
}
