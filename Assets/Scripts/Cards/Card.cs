using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private Transform cardTransform;
    [SerializeField] private Renderer cardRenderer;
    
    [field: SerializeField]
    public CardContainerType ContainerType { get; set; }
    
    private CardData.CardState _currentState = CardData.CardState.Normal;
    
    private static int _globalCardId = -1;
    
    public int CardId { get; private set; }
    
    public Vector3 VisualPosition
    {
        get => cardTransform.position;
        set => cardTransform.position = value;
    }

    public bool IsTokenVisible
    {
        get => textMesh.gameObject.activeSelf;
        set => textMesh.gameObject.SetActive(value);
    }

    public CardData.TokenType TokenType { get; private set; } = CardData.TokenType.IllegalToken;
    public string Token
    {
        get => textMesh.text;
        set
        {
            if (DetermineTokenType(value))
            {
                textMesh.text = value;
            }
            else
            {
                Debug.Log($"Invalid token type: {value}", gameObject);
            }
        }
    }

    public CardData.CardState State
    {
        get => _currentState;
        set
        {
            _currentState = value;
            UpdateCardColor();
            UpdateCardTag();
        }
    }

    public void Initialize(string token = "0", bool isTokenVisible = true,
        CardData.CardState state = CardData.CardState.Normal)
    {
        CardId = GenerateUniqueID();
        Token = token;
        IsTokenVisible = isTokenVisible;
        State = state;
        UpdateCardColor();
    }
    
    private void UpdateCardColor()
    {
        if (cardRenderer != null)
        {
            Color newColor = cardData.GetColorForState(_currentState);
            cardRenderer.material.color = newColor;
        }
        else
        {
            Debug.LogError($"{nameof(Card)} has no card renderer.", gameObject);
        }
    }

    private void UpdateCardTag()
    {
        transform.tag = State == CardData.CardState.NonPickable ? "NonPickableCard" : "Card";
    }

    private int GenerateUniqueID()
    {
        return ++_globalCardId;
    }

    private bool DetermineTokenType(string token)
    {
        if (int.TryParse(token, out var numericValue))
        {
            switch (token.Length)
            {
                case 1:
                    TokenType = CardData.TokenType.SingleDigit;
                    break;
                case 2:
                    TokenType = CardData.TokenType.DoubleDigit;
                    break;
                default:
                    TokenType = CardData.TokenType.IllegalToken;
                    return false;
            }
        }
        else if (token is "+" or "-" or "*" or "/")
        {
            TokenType = CardData.TokenType.Symbol;
        }
        else
        {
            TokenType = CardData.TokenType.IllegalToken;
            return false;
        }
        
        return true;
    }
}
