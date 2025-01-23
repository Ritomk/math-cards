using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class RpnExpressionHelper
{
    public static string ExpressionToString(List<float> expression)
    {
        List<string> tokensAsString = new List<string>();
        foreach (float token in expression)
        {
            if (TokenMapping.FloatToStringMap.TryGetValue(token, out string opStr))
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
    
    public static bool IsOperator(float token) => token is >= 0.05f and <= 0.08f;
    
    public static bool EvaluateRpnExpressionOrder(
        Dictionary<int, Card> cardsDictionary,
        out float result,
        out List<List<int>> operationOrder)
    {
        Stack<float> resultStack = new Stack<float>();
        Stack<int> idStack = new Stack<int>();
        operationOrder = new List<List<int>>();

        foreach (var (cardId, card) in cardsDictionary)
        {
            // card.Token is float
            if (!IsOperator(card.Token))
            {
                // Push numeric token
                resultStack.Push(card.Token);
                idStack.Push(cardId);
            }
            else
            {
                // Operator => pop two floats
                if (resultStack.Count < 2 || idStack.Count < 2)
                {
                    Debug.LogError("Not enough operands on the stack for the operation.");
                    result = 0;
                    return false;
                }

                float operand2 = resultStack.Pop();
                float operand1 = resultStack.Pop();
                
                int operand2Id = idStack.Pop();
                int operand1Id = idStack.Pop();

                float operationResult = ApplyOperator(operand1, operand2, card.Token);

                // Push result back
                resultStack.Push(operationResult);
                idStack.Push(cardId);

                // Record the operation: { OperatorCardID, Operand1CardID, Operand2CardID }
                var currentOperation = new List<int> { cardId, operand1Id, operand2Id };
                operationOrder.Add(currentOperation);
            }
        }
        
        // Final result is top of the stack
        if (resultStack.Count > 0)
        {
            result = resultStack.Max();
            
            if (result % 1 != 0)
                result = Mathf.Floor(result * 100) / 100;
            Debug.Log($"Final Result: {result}");
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }

    private static Stack<float> EvaluateRpnToStack(List<float> tokens, out bool success)
    {
        var stack = new Stack<float>();
        success = true;

        foreach (var token in tokens)
        {
            if (!IsOperator(token))
            {
                stack.Push(token);
            }
            else
            {
                if (stack.Count < 2)
                {
                    success = false;
                    return stack;
                }

                float b = stack.Pop();
                float a = stack.Pop();
                float res = ApplyOperator(a, b, token);
                stack.Push(res);
            }
        }

        return stack;
    }


    public static bool EvaluateRpnExpression(List<float> expression, out float result)
    {
        var finalStack = EvaluateRpnToStack(expression, out var success);
        if (!success || finalStack.Count == 0)
        {
            result = 0;
            return false;
        }

        if (finalStack.Count == 1)
        {
            result = finalStack.Pop();
            
            if(result % 1 != 0)
                result = Mathf.Floor(result * 100) / 100;
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }
    
    public static bool EvaluateRpnExpressionLeader(List<float> expression, out float result)
    {
        var finalStack = EvaluateRpnToStack(expression, out var success);
        if (!success || finalStack.Count == 0)
        {
            result = 0;
            return false;
        }
        Debug.Log(finalStack.Count);
        result = finalStack.Max();
        if(result % 1 != 0)
            result = Mathf.Floor(result * 100) / 100;
        return true;
    }

    public static bool EvaluateRpnExpressionAll(List<float> expression, out List<float> finalValues)
    {
        Stack<float> stack = EvaluateRpnToStack(expression, out bool success);

        if (!success || stack.Count == 0)
        {
            finalValues = new List<float>();
            return false;
        }
        
        finalValues = stack.Reverse().ToList();

        for (int i = 0; i < finalValues.Count; i++)
        {
            float val = finalValues[i];
            
            if (val % 1 != 0)
                val = Mathf.Floor(val * 100) / 100;
            
            finalValues[i] = val;
        }
        
        return true;
    }

    public static float ApplyOperator(float a, float b, float  opToken)
    {
        switch (opToken)
        {
            case 0.05f:
                return a + b;
            case 0.06f:
                return a - b;
            case 0.07f:
                return a * b;
            case 0.08f:
                if (b == 0) return a / 0.1f;
                return a / b;
            default:
                throw new ArgumentException($"Invalid operator token: {opToken}");
        }
    }
}

public static class TokenMapping
{
    public static readonly Dictionary<float, string> FloatToStringMap =
        new Dictionary<float, string>
        {
            { 0.05f, "+" },
            { 0.06f, "-" },
            { 0.07f, "\u00d7" },
            { 0.08f, "\u00f7" }
        };
    
    public static readonly Dictionary<int, float> IntToFloatMap =
        new Dictionary<int, float>
        {
            { 1, 0.05f },
            { 2, 0.06f },
            { 3, 0.07f },
            { 4, 0.08f }
        };
}