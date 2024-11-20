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