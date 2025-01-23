using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{
    [Category("Card Conditions")]
    public class CheckPlayerHasEndedRound : ConditionTask
    {
        [BlackboardOnly] public BBParameter<SoGameStateEvents> gameStateEvents = new BBParameter<SoGameStateEvents>()
            { name = "Game State Event" };
        public bool isTrue = true;


        protected override string info
        {
            get { return $"Check if value playerHasEndedRound == {isTrue}"; }
        }

        protected override bool OnCheck()
        {
            return gameStateEvents.value.playerHasEndedRound == isTrue;
        }
    }
}