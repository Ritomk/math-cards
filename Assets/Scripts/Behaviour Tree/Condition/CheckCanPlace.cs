using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("Card Conditions")]
    public class CheckCanPlace : ConditionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };
        
        public int comparisonValue;
        public ContainerKey targetContainer;


        protected override string info
        {
            get { return $"Check if value in {targetContainer.OwnerType} {targetContainer.ContainerType} < {comparisonValue}"; }
        }

        protected override bool OnCheck()
        {
            var data = knowledgeData.value;
            if (data == null)
            {
                Debug.LogError("EnemyKnowledgeData is null");
                return false;
            }

            // Retrieve the container values
            int valueInContainer = GetContainerValue(data, targetContainer);

            // Perform the comparison
            return valueInContainer < comparisonValue;
        }
        
        private int GetContainerValue(EnemyKnowledgeData data, ContainerKey key)
        {
            if (targetContainer.Equals(new ContainerKey(OwnerType.Player, CardContainerType.AttackTable)))
            {
                return knowledgeData.value.playerAttackTableList.Count;
            }
            else if (targetContainer.Equals(new ContainerKey(OwnerType.Player, CardContainerType.DefenceTable)))
            {
                return knowledgeData.value.playerDefenceTableList.Count;

            }
            else if (targetContainer.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.AttackTable)))
            {
                return knowledgeData.value.selfAttackTableList.Count;

            }
            else if (targetContainer.Equals(new ContainerKey(OwnerType.Enemy, CardContainerType.DefenceTable)))
            {
                return knowledgeData.value.selfDefenceTableList.Count;
            }

            // Return 0 if container is not found or empty
            Debug.LogWarning($"Container {key} not found. Returning default value 0.");
            return 0;
        }
    }
}