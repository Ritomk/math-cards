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
        private SoCardEvents _opponentSoCardEvents;
        private SoAnimationEvents _soAnimationEvents;

        private int _numberOfCards = 20;
        private float _timeBetweenDraws = 0.3f;

        public BeginRoundState(StateMachine<GameStateEnum> stateMachine, SoGameStateEvents soGameStateEvents,
            SoCardEvents soCardEvents, SoCardEvents opponentSoCardEvents,
            SoAnimationEvents soAnimationEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
            _opponentSoCardEvents = opponentSoCardEvents;
            _soAnimationEvents = soAnimationEvents;
        }

        public override IEnumerator Enter()
        {
            _soAnimationEvents.RaiseToggleChestAnimation(OwnerType.Any, false);
            _soGameStateEvents.RaisePlayerStateChange(PlayerStateEnum.BeginRound);
            
            yield return CoroutineHelper.StartAndWait(DrawCardsWithDelay());
            
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        }

        private IEnumerator DrawCardsWithDelay()
        {
            var shouldContinuePlayer = true;
            var shouldContinueOpponent = true;
            
            for (int i = 0; i < _numberOfCards; i++)
            {
                shouldContinuePlayer = TryDrawCard(_soCardEvents, shouldContinuePlayer);
                shouldContinueOpponent = TryDrawCard(_opponentSoCardEvents, shouldContinueOpponent);
                
                if(!shouldContinuePlayer && !shouldContinueOpponent)
                    yield break;
                
                yield return new WaitForSecondsPauseable(_timeBetweenDraws);
            }
        }

        private bool TryDrawCard(SoCardEvents soCardEvents, bool canStillDraw) =>
            canStillDraw && soCardEvents.RaiseCardDraw(true);
    }
}