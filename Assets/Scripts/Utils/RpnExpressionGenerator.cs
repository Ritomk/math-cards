using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodeCanvas.Framework;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class RpnExpressionGenerator : MonoBehaviour
{
    public event Action<List<int>, float> OnComputationComplete;
    
    private static void ConvertCardsToTokens(Dictionary<int, Card> cards, out List<int> operands,
        out List<int> operators)
    {
        operands = new List<int>();
        operators = new List<int>();

        foreach (var card in cards)
        {
            string tokenStr = card.Value.Token;

            if (TokenMapping.StringToIntMap.TryGetValue(tokenStr, out int tokenInt))
            {
                operators.Add(tokenInt);
            }
            else if (int.TryParse(tokenStr, out int operand))
            {
                operands.Add(operand);
            }
            else
            {
                Debug.LogError($"Invalid token: {tokenStr}");
            }
        }
    }
    
    private static List<List<int>> GenerateAllRpnExpressions(List<int> operands, List<int> operators, int maxLength)
    {
        List<List<int>> expressions = new List<List<int>>();
        GenerateExpressionsRecursive(new List<int>(), operands, operators, 0, expressions, 
            maxLength);
        return expressions;
    }

    private static void GenerateExpressionsRecursive(List<int> currentExpression, List<int> operandsRemaining,
        List<int> operatorsRemaining, int stackHeight, List<List<int>> expressions, int maxLength)
    {
        
        if (currentExpression.Count == maxLength)
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

    private static float ApplyOperator(float a, float b, int opToken)
    {
        switch (opToken)
        {
            case 101:
                return a + b;
            case 102:
                return a - b;
            case 103:
                return a * b;
            case 104:
                if (b == 0) return a / 0.1f;
                return a / b;
            default:
                throw new ArgumentException($"Invalid operator token: {opToken}");
        }
    }

    private static bool IsOperator(int opToken)
    {
        return opToken >= 101;
    }
 
    
    private static bool EvaluateRpnExpression(List<int> expression, out float result)
    {
        Stack<float> stack = new Stack<float>();

        foreach (var token in expression)
        {
            if (!IsOperator(token))
            {
                stack.Push(token);
            }
            else
            {
                if (stack.Count < 2)
                {
                    result = 0;
                    return false;
                }
                float b = stack.Pop();
                float a = stack.Pop();
                float res = ApplyOperator(a, b, token);
                stack.Push(res);
            }
        }

        if (stack.Count == 1)
        {
            result = stack.Pop();
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }
    
    public static string ExpressionToString(List<int> expression)
    {
        List<string> tokensAsString = new List<string>();
        foreach (int token in expression)
        {
            if (TokenMapping.IntToStringMap.TryGetValue(token, out string opStr))
            {
                tokensAsString.Add(opStr);
            }
            else
            {
                tokensAsString.Add(token.ToString());
            }
        }
        return string.Join(" ", tokensAsString);
    }

    public async Task<List<int>> StartComputation(Dictionary<int, Card> cards, int maxLength)
    {
        ConvertCardsToTokens(cards, out List<int> operands, out List<int> operators);

        var expression = await Task.Run(() =>
        {
            List<List<int>> expressions = GenerateAllRpnExpressions(operands, operators, maxLength);
            
            float maxResult = float.MinValue;
            List<int> maxExpression = null;
            
            foreach (var expr in expressions)
            {
                if (EvaluateRpnExpression(expr, out float result))
                {
                    if (result > maxResult)
                    {
                        maxResult = result;
                        maxExpression = expr;
                    }
                }
            }

            return maxExpression ?? new List<int>();
        });
        
        return expression;
    }
}

public static class TokenMapping
{
    public static readonly Dictionary<string, int> StringToIntMap = new Dictionary<string, int>
    {
        { "+", 101 },
        { "-", 102 },
        { "\u00d7", 103 },
        { "\u00f7", 104 }
    };
    
    public static readonly Dictionary<int, string> IntToStringMap = new Dictionary<int, string>
    {
        { 101, "+" },
        { 102, "-" },
        { 103, "\u00d7" },
        { 104, "\u00f7" },
    };
}