using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions {

	[Category("Card Tasks")]
	public class PlaceCard : ActionTask
	{
		[BlackboardOnly] public BBParameter<Dictionary<int, Card>> _cards;
		[BlackboardOnly] public BBParameter<SoCardEvents> _soCardEvents;
		

		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit() {
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute()
		{
			var from = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);
			var to = new ContainerKey(OwnerType.Enemy, CardContainerType.AttackTable);
			_soCardEvents.value.RaiseCardMove(_cards.value.First().Value, from, to);
			
			EndAction(true);
		}

		//Called once per frame while the action is active.
		protected override void OnUpdate() {
			
		}

		//Called when the task is disabled.
		protected override void OnStop() {
			
		}

		//Called when the task is paused.
		protected override void OnPause() {
			
		}
	}
}