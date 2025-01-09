using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RpnExpressionGenerator : MonoBehaviour
{
    private static void ConvertCardsToTokens(Dictionary<int, Card> cards, out List<int> operands,
        out List<int> operators)
    {
        operands = new List<int>();
        operators = new List<int>();

        foreach (var card in cards)
        {
            int token = card.Value.Token;

            if (IsOperator(token))
            {
                operators.Add(token);
            }
            else
            {
                operands.Add(token);
            }
        }
    }

    private static (List<int>, float) GenerateAndEvaluateExpressions(List<int> initialExpression,
        List<int> initialGroup, List<int> operands, List<int> operators, int maxLength)
    {
        List<List<int>> expressions = new List<List<int>>();

        int initialStackHeight = CalculateInitialStackHeight(initialExpression);

        GenerateExpressionsRecursive(new List<int>(initialExpression), operands, operators,
            initialStackHeight, expressions, maxLength);
        
        /*foreach (var initialToken in initialGroup)
        {
            var remainingOperands = new List<int>(operands);
            remainingOperands.Remove(initialToken);

            var groupExpression = new List<int>(initialExpression) { initialToken };

            var stackChange = IsOperator(initialToken) ? -1 : 1;
            Debug.Log($"Debug: Group expression {string.Join(" ", groupExpression)}");
            Debug.Log($"Debug: Stack change {initialStackHeight + stackChange}");
            Debug.Log($"Debug: Max length {maxLength}");
            GenerateExpressionsRecursive(groupExpression, remainingOperands, operators,
                 initialStackHeight + stackChange, expressions, maxLength);
        }*/

        float maxResult = float.MinValue;
        List<int> maxExpression = null;

        foreach (var expression in expressions)
        {
            if (RpnExpressionHelper.EvaluateRpnExpression(expression, out float result))
            {
                if (result > maxResult)
                {
                    maxResult = result;
                    maxExpression = expression;
                }
            }
        }

        return (maxExpression ?? new List<int>(), maxResult);
    }

    private static void GenerateExpressionsRecursive(List<int> currentExpression, List<int> operandsRemaining,
        List<int> operatorsRemaining, int stackHeight, List<List<int>> expressions, int maxLength)
    {
        if (currentExpression.Count >= maxLength)
        {
            // Check if we have a valid expression (stackHeight == 1)
            if (stackHeight == 1)
            {
                expressions.Add(new List<int>(currentExpression));
            }
            return;
        }

        if (operandsRemaining.Count > 0)
        {
            for (int i = 0; i < operandsRemaining.Count; ++i)
            {
                int operand = operandsRemaining[i];
                operandsRemaining.RemoveAt(i);
                currentExpression.Add(operand);

                GenerateExpressionsRecursive(currentExpression, operandsRemaining, operatorsRemaining,
                    stackHeight + 1, expressions, maxLength);

                // Backtrack
                currentExpression.RemoveAt(currentExpression.Count - 1);
                operandsRemaining.Insert(i, operand);   
            }
        }

        if (stackHeight >= 2 && operatorsRemaining.Count > 0)
        {
            for (int i = 0; i < operatorsRemaining.Count; ++i)
            {
                int op = operatorsRemaining[i];
                operatorsRemaining.RemoveAt(i);
                currentExpression.Add(op);
                
                GenerateExpressionsRecursive(currentExpression, operandsRemaining, operatorsRemaining,
                    stackHeight - 1, expressions, maxLength);
                
                //Backtrack
                currentExpression.RemoveAt(currentExpression.Count - 1);
                operatorsRemaining.Insert(i, op);
            }
        }
    }
    
    private static int CalculateInitialStackHeight(List<int> initialExpression)
    {
        int stackHeight = 0;

        foreach (var token in initialExpression)
        {
            if (IsOperator(token))
            {
                stackHeight -= 1;
            }
            else
            {
                stackHeight += 1;
            }

            if (stackHeight < 0)
            {
                throw new InvalidOperationException("Invalid initial expression: stackHeight became negative.");
            }
        }
        
        return stackHeight;
    }

    private static bool IsOperator(int opToken)
    {
        return opToken >= 101;
    }
    
    private static void PrepareGroups(List<int> initialExpression, List<int> operands, List<int> operators,
        out List<int> group1, out List<int> group2)
    {
        group1 = new List<int>();
        group2 = new List<int>();
        
        int initialStackHeight = CalculateInitialStackHeight(initialExpression);
        
        int groupSize = operands.Count / 2;
        int remainder = operands.Count % 2;
        
        group1 = operands.GetRange(0, groupSize + (remainder > 0 ? 1 : 0));
        group2 = operands.GetRange(group1.Count, operands.Count - group1.Count);

        if (initialExpression.Count > 2 && initialStackHeight > 1)
        {
            var groupOperators1 = operators.GetRange(0, groupSize + (remainder > 0 ? 1 : 0));
            var groupOperators2 = operators.GetRange(group1.Count, groupSize + (remainder > 0 ? 1 : 0));
            
            group1.AddRange(groupOperators1);
            group1.AddRange(groupOperators2);
        }
    }

    public async Task<List<int>> StartComputation(List<int> initialGroup, Dictionary<int, Card> cards, int maxLength)
    {
        ConvertCardsToTokens(cards, out List<int> operands, out List<int> operators);
        
        PrepareGroups(new List<int>(), operands, operators, out List<int> group1, out List<int> group2);
        
        var task1 = Task.Run(() => GenerateAndEvaluateExpressions(initialGroup, group1, operands,
            new List<int>(operators), maxLength));
        //var task2 = Task.Run(() => GenerateAndEvaluateExpressions(initialGroup, group2, operands,
        //    new List<int>(operators), maxLength));
        
        var results = await Task.WhenAll(task1);

        var bestExpression = results
            .OrderByDescending(result => result.Item2)
            .First().Item1;
        
        return bestExpression;
    }
}