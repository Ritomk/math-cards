using System.Collections;
using System.Collections.Generic;
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

            _behaviourTreeOwner.enabled = true;
            _soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.OpponentTurnIdle);

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
