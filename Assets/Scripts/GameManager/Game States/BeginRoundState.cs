using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class BeginRoundState : GameStateBase
    {
        public override GameStateEnum StateType => GameStateEnum.BeginRound;

        private SoGameStateEvents _soGameStateEvents;
        private SoCardEvents _soCardEvents;
        private SoCardEvents _enemySoCardEvents;

        private int _numberOfCards = 20;
        private float _timeBetweenDraws = 0.3f;


        public BeginRoundState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents, SoCardEvents soCardEvents, SoCardEvents enemySoCardEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
            _enemySoCardEvents = enemySoCardEvents;
        }

        public override IEnumerator Enter()
        {
            CoroutineHelper.Start(DrawCardsWithDelay());
            
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);

            yield return null;
        }

        private IEnumerator DrawCardsWithDelay()
        {
            for (int i = 0; i < _numberOfCards; i++)
            {
                _soCardEvents.RaiseCardDraw();
                _enemySoCardEvents.RaiseCardDraw();
                yield return new WaitForSecondsPauseable(_timeBetweenDraws);
            }
        }
    }
}