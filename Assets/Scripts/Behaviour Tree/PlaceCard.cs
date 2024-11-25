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
		
		[BlackboardOnly] public BBParameter<Stack<Card>> tableMoves = new BBParameter<Stack<Card>>()
			{ name = "Attack Table Moves" };

		public ContainerKey targetContainer;

		private static readonly ContainerKey handContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);
		


		protected override void OnExecute()
		{
			if (tableMoves.value.Count != 0)
			{
				var card = tableMoves.value.Pop();

				if (soCardEvents.value.RaiseCardMove(card, handContainer, targetContainer) &&
				    knowledgeData.value.selfHandCardsDictionary.Remove(card.CardId))
				{
					EndAction(true);
				}
			}

			EndAction(false);
		}
	}
}