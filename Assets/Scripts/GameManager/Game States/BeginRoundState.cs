using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStates
{
    public class BeginRoundState : GameStateBase
    {
        private SoGameStateEvents _soGameStateEvents;
        private SoCardEvents _soCardEvents;

        private int _numberOfCards = 5;
        private float _timeBetweenDraws = 0.3f;


        public BeginRoundState(GameStateMachine stateMachine, SoGameStateEvents soGameStateEvents, SoCardEvents soCardEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
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
                yield return new WaitForSecondsPauseable(_timeBetweenDraws);
            }
        }
    }
}