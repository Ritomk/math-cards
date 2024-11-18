using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions 
{

	[Category("Card Tasks")]
	public class GetCardsDataFromContainer : ActionTask<CardContainerBase>
	{
		[BlackboardOnly] public readonly BBParameter<SoContainerEvents> ContainerEvents =
			new BBParameter<SoContainerEvents>() { name = "Container Events" };

		[BlackboardOnly] public BBParameter<EnemyKnowledgeData> EnemyKnowledgeData =
			new BBParameter<EnemyKnowledgeData>() { name = "Enemy Knowledge Data" };

		[BlackboardOnly] public BBParameter<int> SelfHandCardsCount =
			new BBParameter<int>() { name = "Self Hand Cards Count" };


		//Use for initialization. This is called only once in the lifetime of the task.
		//Return null if init was successfull. Return an error string otherwise
		protected override string OnInit() {
			return null;
		}

		//This is called once each time the task is enabled.
		//Call EndAction() to mark the action as finished, either in success or failure.
		//EndAction can be called from anywhere.
		protected override void OnExecute() {
			if (agent == null)
			{
				Debug.LogError("Ageint is null in GetCardsDataFromContainer task.");
				EndAction(false);
				return;
			}

			EnemyKnowledgeData.value = ContainerEvents.value.RaiseGetCardData();
			SelfHandCardsCount.value = EnemyKnowledgeData.value.selfHandCardsDictionary.Count;
			
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