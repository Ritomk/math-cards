using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions {

    [Category("Card Tasks")]
    public class MergeCards : ActionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };

        [BlackboardOnly] public BBParameter<SoCardEvents> soCardEvents = new BBParameter<SoCardEvents>()
            { name = "Card Events" };
        
        [BlackboardOnly] public BBParameter<bool> hasStartedMerging = new BBParameter<bool>()
            {name = "Has Started Merging"};

        private readonly ContainerKey targetContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Merger);

        private readonly ContainerKey handContainer = new ContainerKey(OwnerType.Enemy, CardContainerType.Hand);

        // protected override string info
        // {
        //     get { return $"Place Card on {targetContainer.OwnerType} {targetContainer.ContainerType}"; }
        // }

        protected override void OnExecute()
        {
            var cardsDictionary = knowledgeData.value.selfHandCardsDictionary;
            var card = PickRandomSingleDigitCard(cardsDictionary);
            
            if (soCardEvents.value.RaiseCardMove(card, handContainer, targetContainer) &&
                knowledgeData.value.selfHandCardsDictionary.Remove(card.CardId))
            {
                hasStartedMerging.value = !hasStartedMerging.value;
                EndAction(true);
            }

            EndAction(false);
        }

        private Card PickRandomSingleDigitCard(Dictionary<int, Card> cardsDictionary)
        {
            var singleDigitCards = cardsDictionary.Values
                .Where(card => card.TokenType == CardData.TokenType.SingleDigit
                && (hasStartedMerging.value || card.Token != 0))
                .ToList();

            if (!singleDigitCards.Any())
            {
                Debug.LogWarning("No cards with TokenType.SingleDigit available in the container.");
                return null;
            }

            var weightedCards = new List<(Card card, int cumulativeWeight)>();
            int cumulativeWeight = 0;

            foreach (var card in singleDigitCards)
            {
                cumulativeWeight += card.TokenWeight;
                weightedCards.Add((card, cumulativeWeight));
            }

            int randomValue = UnityEngine.Random.Range(0, cumulativeWeight);

            foreach (var (card, weight) in weightedCards)
            {
                if (randomValue < weight)
                {
                    return card;
                }
            }

            Debug.LogError("Failed to pick a card due to unexpected logic error.");
            return null;
        }
    }
}