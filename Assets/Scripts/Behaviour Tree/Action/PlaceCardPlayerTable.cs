using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Actions {

    [Category("Card Tasks")]
    public class PlaceCardPlayerTable : ActionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };

        [BlackboardOnly] public BBParameter<SoCardEvents> soCardEvents = new BBParameter<SoCardEvents>()
            { name = "So Card Events" };
        
        [BlackboardOnly] public BBParameter<Card> chosenCardToPlace = new BBParameter<Card>()
            { name = "Chosen Card To Place" };
		
        [BlackboardOnly] public BBParameter<int> selfHandCardsCount = new BBParameter<int>()
            { name = "Self Hand Cards Count" };

        public ContainerKey targetContainer;

        private readonly ContainerKey handContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);
		
        protected override string info
        {
            get { return $"Place Card on {targetContainer.OwnerType} {targetContainer.ContainerType}"; }
        }

        protected override void OnExecute()
        {
            if (chosenCardToPlace != null)
            {
                var card = chosenCardToPlace.value;

                if (soCardEvents.value.RaiseCardMove(card, handContainer, targetContainer) &&
                    knowledgeData.value.selfHandCardsDictionary.Remove(card.CardId))
                    selfHandCardsCount.value--;
					
                if (targetContainer.Equals(new ContainerKey(OwnerType.Player, CardContainerType.AttackTable)))
                {
                    knowledgeData.value.selfAttackTableList.Add(card.Token);
                }
                else if (targetContainer.Equals(new ContainerKey(OwnerType.Player, CardContainerType.DefenceTable)))
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