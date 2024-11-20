using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions 
{

	[Category("Card Tasks")]
	public class GetCardsDataFromContainer : ActionTask<CardContainerBase>
	{
		[BlackboardOnly] public readonly BBParameter<SoContainerEvents> containerEvents =
			new BBParameter<SoContainerEvents>() { name = "Container Events" };

		[BlackboardOnly] public BBParameter<EnemyKnowledgeData> enemyKnowledgeData =
			new BBParameter<EnemyKnowledgeData>() { name = "Enemy Knowledge Data" };
		
		[BlackboardOnly] public BBParameter<Dictionary<int, Card>> availableCardsPool =
			new BBParameter<Dictionary<int, Card>>() { name = "Available Cards Pool" };

		[BlackboardOnly] public BBParameter<int> selfHandCardsCount =
			new BBParameter<int>() { name = "Self Hand Cards Count" };
		
		[BlackboardOnly] public BBParameter<List<string>> selfAttackTableList =
			new BBParameter<List<string>>() { name = "Self Attack Table List" };
		[BlackboardOnly] public BBParameter<List<string>> selfDefenceTableList =
			new BBParameter<List<string>>() { name = "Self Defence Table List" };

		[BlackboardOnly] public BBParameter<bool> generateMovesAttack = new BBParameter<bool>()
			{name = "Generate Moves Attack"};
		[BlackboardOnly] public BBParameter<bool> generateMovesDefence = new BBParameter<bool>()
			{name = "Generate Moves Defence"};
		[BlackboardOnly] public BBParameter<bool> generateCardsPool = new BBParameter<bool>()
			{name = "Generate Cards Pool"};
		
		protected override void OnExecute() {
			if (agent == null)
			{
				Debug.LogError("Agent is null in GetCardsDataFromContainer task.");
				EndAction(false);
				return;
			}

			var newData = FetchCardData();
			HasTablesChanged(newData.selfAttackTableList, newData.selfDefenceTableList);
			UpdateKnowledgeData(newData);
			CreateCardsPool();
			
			EndAction(true);
		}

		private EnemyKnowledgeData FetchCardData()
		{
			return containerEvents.value.RaiseGetCardData();
		}

		private void HasTablesChanged(List<string> oldAttackTable, List<string> oldDefenceTable)
		{
			if (selfAttackTableList.value == null ||
			    !selfAttackTableList.value.SequenceEqual(oldAttackTable))
			{
				generateMovesAttack.value = true;
			}

			if (selfDefenceTableList.value == null ||
			    !selfDefenceTableList.value.SequenceEqual(oldDefenceTable))
			{
				generateMovesDefence.value = true;
			}
		}

		private void UpdateKnowledgeData(EnemyKnowledgeData newData)
		{
			enemyKnowledgeData.value = newData;
			selfHandCardsCount.value = newData.selfHandCardsDictionary.Count;
			
			selfAttackTableList.value = newData.selfAttackTableList;
			selfDefenceTableList.value = newData.selfDefenceTableList;
		}
		
		private void CreateCardsPool()
		{
			if(generateCardsPool.value == false) return;

			availableCardsPool.value?.Clear();
			availableCardsPool.value = new Dictionary<int, Card>(enemyKnowledgeData.value.selfHandCardsDictionary);
		}
	}
}