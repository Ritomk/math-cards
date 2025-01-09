using UnityEngine;

namespace GameStates
{
    public class GameOverState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.GameOver;
    
        public GameOverState(StateMachine<GameStateEnum> stateMachine) : base(stateMachine)
        {
        }
    }   
}