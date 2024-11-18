using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Card Tasks")]
    public class GenerateMoves : ActionTask
    {
        [BlackboardOnly] public BBParameter<EnemyKnowledgeData> knowledgeData = new BBParameter<EnemyKnowledgeData>()
            { name = "Enemy Knowledge Data" };
        [BlackboardOnly]
        public BBParameter<RpnExpressionGenerator> generator = new BBParameter<RpnExpressionGenerator>();

        public int maxExpressionLength = 5;
        
        protected override void OnExecute()
        {
            GenerateExpression();
        }

        private async void GenerateExpression()
        {
            float startTime = Time.realtimeSinceStartup;
            
            var cards = knowledgeData.value.selfHandCardsDictionary;
            var expression = await generator.value.StartComputation(cards, maxExpressionLength);
            
            float endTime = Time.realtimeSinceStartup;
            
            Debug.Log("AI: Calculation finished");
            Debug.Log($"AI: Elapsed time: {endTime - startTime}");
            Debug.Log($"AI: Expression: {RpnExpressionGenerator.ExpressionToString(expression)}");
            
            EndAction(expression.Count > 0);
        }

    }
}