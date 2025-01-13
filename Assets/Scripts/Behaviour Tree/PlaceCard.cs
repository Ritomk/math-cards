using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


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
		
		[BlackboardOnly] public BBParameter<int> selfHandCardsCount = new BBParameter<int>()
			{ name = "Self Hand Cards Count" };

		public ContainerKey targetContainer;

		private static readonly ContainerKey handContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);
		


		protected override void OnExecute()
		{
			if (tableMoves.value.Count != 0)
			{
				var card = tableMoves.value.Pop();

				if (soCardEvents.value.RaiseCardMove(card, handContainer, targetContainer) &&
				    knowledgeData.value.selfHandCardsDictionary.Remove(card.CardId))
					selfHandCardsCount.value--;
					
					if (targetContainer.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.AttackTable)))
					{
						knowledgeData.value.selfAttackTableList.Add(card.Token);
					}
					else if (targetContainer.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.DefenceTable)))
					{
						knowledgeData.value.selfDefenceTableList.Add(card.Token);
					}
				{
					EndAction(true);
				}
			}

			EndAction(false);
		}
	}
}