using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class RpnExpressionHelper
{
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

    public static List<string> ExpressionToStringList(List<int> expression)
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
        return tokensAsString;
    }
    
    public static List<int> ExpressionToIntList(List<string> expression)
    {
        List<int> tokensAsInt = new List<int>();
        foreach (string token in expression)
        {
            if (TokenMapping.StringToIntMap.TryGetValue(token, out int opInt))
            {
                tokensAsInt.Add(opInt);
            }
            else if (int.TryParse(token, out int number))
            {
                tokensAsInt.Add(number);
            }
            else
            {
                throw new ArgumentException($"Invalid token: {token}");
            }
        }
        return tokensAsInt;
    }
    
    private static bool IsOperator(int opToken)
    {
        return opToken >= 101;
    }
    
    public static bool EvaluateRpnExpression(List<int> expression, out float result)
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

    public static bool EvaluateRpnExpressionOrder(
        Dictionary<int, Card> cardsDictionary,
        out float result,
        out List<List<int>> operationOrder)
    {
        Stack<float> resultStack = new Stack<float>(); // Stack for calculation results
        Stack<int> idStack = new Stack<int>(); // Stack for card IDs
        operationOrder = new List<List<int>>();

        foreach (var (cardId, card) in cardsDictionary)
        {
            if (!IsOperator(card.Token))
            {
                resultStack.Push(card.Token);
                idStack.Push(cardId);
            }
            else
            {
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

                resultStack.Push(operationResult);
                idStack.Push(cardId);

                var currentOperation = new List<int> { cardId, operand1Id, operand2Id };
                operationOrder.Add(currentOperation);
            }
        }
        
        if (resultStack.Count > 0)
        {
            result = resultStack.Max();
            Debug.Log($"Final Result: {result}");
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }
    
    public static bool EvaluateRpnExpressionLeader(List<int> expression, out float result)
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

        if (stack.Count > 0)
        {
            result = stack.Max();
            return true;
        }
        else
        {
            result = 0;
            return false;
        }
    }

    public static int EncodeToken(float value)
    {
        if (!Mathf.Approximately(value, (int)value))
        {
            int sign = value < 0 ? -1 : 1;
            int wholePart = Mathf.FloorToInt(Mathf.Abs(value));
            int fractionalPart = Mathf.FloorToInt((Mathf.Abs(value) - wholePart) * 100);

            int encodedValue = 10000 + (wholePart * 100) + fractionalPart;
            return encodedValue * sign;
        }
        else
        {
            return (int)value;
        }
    }

    public static float ApplyOperator(float a, float b, int opToken)
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