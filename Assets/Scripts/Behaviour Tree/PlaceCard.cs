using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions {

	[Category("Card Tasks")]
	public class PlaceCard : ActionTask
	{
		[BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
			{ name = "Enemy Knowledge Data" };

		[BlackboardOnly] public BBParameter<SoCardEvents> soCardEvents = new BBParameter<SoCardEvents>()
			{ name = "Card Events" };

		public ContainerKey targetContainer;

		private static readonly ContainerKey handContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);

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
			var randomIndex = Random.Range(0, knowledgeData.value.selfHandCardsDictionary.Count);
			Debug.Log(knowledgeData.value.selfHandCardsDictionary.Count);
			var pickedCard = knowledgeData.value.selfHandCardsDictionary.ElementAt(randomIndex).Value;
			
			soCardEvents.value.RaiseCardMove(pickedCard, handContainer, targetContainer);
			
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