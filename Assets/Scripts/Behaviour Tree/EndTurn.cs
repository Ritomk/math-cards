using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions {

	[Category("Card Tasks")]
	public class EndTurn : ActionTask {

		[BlackboardOnly] public BBParameter<SoGameStateEvents> gameStateEvents = new BBParameter<SoGameStateEvents>()
			{ name = "Game State Event" };
		
		protected override string OnInit() {
			return null;
		}

		protected override void OnExecute() {
			gameStateEvents.value.RaiseGameStateChange(GameStateEnum.PlayerTurn);
			EndAction(true);
		}

		protected override void OnUpdate() {
			
		}

		protected override void OnStop() {
			
		}

		protected override void OnPause() {
			
		}
	}
}