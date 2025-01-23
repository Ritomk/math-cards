using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions {

	[Category("Card Tasks")]
	public class EndRound : ActionTask {

		[BlackboardOnly] public BBParameter<SoGameStateEvents> gameStateEvents = new BBParameter<SoGameStateEvents>()
			{ name = "Game State Event" };
		
		protected override string OnInit() {
			return null;
		}


		protected override void OnExecute() {
			gameStateEvents.value.RaiseEndRound(OwnerType.Enemy);
			EndAction(true);
		}
	}
}