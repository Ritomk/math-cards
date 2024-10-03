using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Table : CardContainerBase
{
    [SerializeField] private GameObject cardPrefab;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(EvaluateExpression());
        }
    }

    public override bool AddCard(Card card)
    {
        bool result = base.AddCard(card);
        if (result)
        {
            UpdateCardPositions();
        }
        
        return result;
    }

    private void UpdateCardPositions()
    {
        float spacing = -5.5f;
        int index = 1;
        foreach (var card in cardsDictionary.Values)
        {
            card.transform.localPosition = new Vector3(spacing * index, 0.2f, 0);
            card.transform.rotation = Quaternion.Euler(0, 0, 0);
            ++index;
        }
    }

    public float EvaluateExpression()
    {
        List <string> tokens = new List<string> ();
        foreach (var card in cardsDictionary.Values)
        {
            tokens.Add(card.Token);
        }

        return EvaluateRPN(tokens);
    }

    private float EvaluateRPN(List<string> tokens)
    {
        Stack<float> stack = new Stack<float>();
        foreach (var token in tokens)
        {
            if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                float b = stack.Pop();
                float a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    default: throw new InvalidOperationException($"Invalid operator {token}");
                }
            }
        }
        return stack.Pop();
    }
}
