using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RpnExpressionGenerator : MonoBehaviour
{
    private static void ConvertCardsToTokens(Dictionary<int, Card> cards, out List<float> operands,
        out List<float> operators)
    {
        operands = new List<float>();
        operators = new List<float>();

        foreach (var card in cards)
        {
            float token = card.Value.Token;

            if (RpnExpressionHelper.IsOperator(token))
            {
                operators.Add(token);
            }
            else
            {
                operands.Add(token);
            }
        }
    }

    private static (List<float>, float) GenerateAndEvaluateExpressions(List<float> initialExpression,
        List<float> operands, List<float> operators, int maxLength)
    {
        List<List<float>> expressions = new List<List<float>>();

        int initialStackHeight = CalculateInitialStackHeight(initialExpression);

        GenerateExpressionsRecursive(new List<float>(initialExpression), operands, operators,
             initialStackHeight, expressions, maxLength);

        float maxResult = float.MinValue;
        List<float> maxExpression = null;

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

        return (maxExpression ?? new List<float>(), maxResult);
    }

    private static void GenerateExpressionsRecursive(List<float> currentExpression, List<float> operandsRemaining,
        List<float> operatorsRemaining, int stackHeight, List<List<float>> expressions, int maxLength)
    {
        if (currentExpression.Count >= maxLength)
        {
            // Check if we have a valid expression (stackHeight == 1)
            if (stackHeight == 1)
            {
                expressions.Add(new List<float>(currentExpression));
            }
            return;
        }

        if (operandsRemaining.Count > 0)
        {
            for (int i = 0; i < operandsRemaining.Count; ++i)
            {
                var operand = operandsRemaining[i];
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
                var op = operatorsRemaining[i];
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
    
    private static int CalculateInitialStackHeight(List<float> initialExpression)
    {
        int stackHeight = 0;

        foreach (var token in initialExpression)
        {
            if (RpnExpressionHelper.IsOperator(token))
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

    public async Task<List<float>> StartComputation(List<float> initialGroup, Dictionary<int, Card> cards, int maxLength)
    {
        ConvertCardsToTokens(cards, out List<float> operands, out List<float> operators);
        
        var task1 = Task.Run(() => GenerateAndEvaluateExpressions(initialGroup, operands,
            new List<float>(operators), maxLength));
        
        var results = await Task.WhenAll(task1);

        var bestExpression = results
            .OrderByDescending(result => result.Item2)
            .First().Item1;
        
        return bestExpression;
    }
}