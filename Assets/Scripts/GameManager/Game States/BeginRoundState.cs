namespace GameStates
{
    public class BeginRoundState : GameStateBase
    {
        private SoGameStateEvents _soGameStateEvents;
        private SoCardEvents _soCardEvents;


        public BeginRoundState(GameStateMachine stateMachine, SoGameStateEvents soGameStateEvents, SoCardEvents soCardEvents) : base(stateMachine)
        {
            _soGameStateEvents = soGameStateEvents;
            _soCardEvents = soCardEvents;
        }

        public override void Enter()
        {
            for (int i = 0; i < 5; i++)
            {
                _soCardEvents.RaiseCardDraw();
            }
            
            _soGameStateEvents.RaiseGameStateChange(GameStateEnum.PlayerTurn);
        }
    }
}