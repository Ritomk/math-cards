using System;
using System.Collections.Generic;


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