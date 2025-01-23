using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Globalization;
using Unity.Mathematics;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;

    [Header("UI Elements")] 
    [SerializeField] private GameObject frontCard;
    [SerializeField] private GameObject backCard;
    [SerializeField] private Texture2D[] textures;
    [SerializeField] private TextMeshPro tokenText;
    [SerializeField] private TextMeshPro duplicatesText;
    
    [Header("Shader controllers")]
    [SerializeField] private DissolveEffect dissolveShader;
    [SerializeField] private HighlightEffect highlightShader;
    
    [field: Header("Card data")]
    [field: SerializeField] public ContainerKey ContainerKey { get; set; }
    
    [SerializeField] private CardData.CardState _currentState = CardData.CardState.Normal;
    
    private static int _globalCardId = -1;
    
    public int CardId { get; private set; }
    
    public bool IsTokenVisible
    {
        get => tokenText.gameObject.activeSelf;
        set => tokenText.gameObject.SetActive(value);
    }

    [field: SerializeField] public CardData.TokenType TokenType { get; private set; } = CardData.TokenType.IllegalToken;

    private float _token;
    
    public float Token
    {
        get => _token;
        set
        {
            if (DetermineTokenType(value))
            {
                DetermineTokenWeight(value);
                _token = value;
                UpdateTextMesh();
            }
            else
            {
                tokenText.text = "Error"; 
                Debug.LogError($"Invalid token type: {value}", gameObject);
            }
        }
    }

    public int TokenWeight { get; private set; } = 1; 
    
    public CardData.CardState State
    {
        get => _currentState;
        set
        {
            _currentState = value;
            UpdateCardColor();
            UpdateCardTag();

            IsTokenVisible = _currentState is not (CardData.CardState.NonPickable or CardData.CardState.EnemyHand);
            DrawFrontCard = _currentState != CardData.CardState.EnemyHand;
        }
    }

    public int Duplicates
    {
        get => _duplicates;
        set
        {
            _duplicates = value;

            if (State != CardData.CardState.EnemyHand)
            {
                var texture = value > 1 ? textures[1] : textures[0];
                UpdateTexture(texture);
            }
        }
    }
    private int _duplicates;

    public bool DrawFrontCard
    {
        get => _drawFrontCard;
        set
        {
            _drawFrontCard = value;
            frontCard.SetActive(_drawFrontCard);
            backCard.SetActive(!_drawFrontCard);
        }
    }
    private bool _drawFrontCard;

    public void Initialize(float token = 0, bool isTokenVisible = true,
        CardData.CardState state = CardData.CardState.Normal, int duplicates = 0, bool drawFrontCard = true)
    {
        CardId = GenerateUniqueID();
        Token = token;
        IsTokenVisible = isTokenVisible;
        State = state;
        Duplicates = duplicates;
        DrawFrontCard = drawFrontCard;
    }
    
    public IEnumerator DissolveAndDestroy()
    {
        yield return CoroutineHelper.StartAndWait(dissolveShader.StartDissolve());
        Destroy(gameObject);
    }

    public void AddCardToHand()
    {
        var baseColorSet = cardData.GetColorSetForState(CardData.CardState.Normal);
        var changeColorSet = cardData.GetColorSetForState(CardData.CardState.Highlighted);

        highlightShader.AddCardToHand(baseColorSet.outlineColor, baseColorSet.highlightColor,
            changeColorSet.outlineColor, changeColorSet.highlightColor, 0.6f);
    } 

    private void UpdateTextMesh()
    {
        tokenText.text = TokenType switch
        {
            CardData.TokenType.Symbol => OperatorToString(_token),
            CardData.TokenType.SingleDigit or CardData.TokenType.ManyDigits => _token.ToString("0"),
            CardData.TokenType.Float => _token.ToString("0.00"),
            _ => tokenText.text
        };
    }
    
    private void UpdateCardColor()
    {
        var colorSet = cardData.GetColorSetForState(_currentState);

        dissolveShader.ChangeColor(colorSet.color);
        
        if(highlightShader.gameObject.activeSelf != colorSet.hasHighlight)
            highlightShader.gameObject.SetActive(colorSet.hasHighlight);
        
        if (colorSet.hasHighlight)
        {
            highlightShader.SmoothChangeHighlightColor(colorSet.outlineColor, 0.2f);
            highlightShader.SmoothChangeOutlineColor(colorSet.highlightColor, 0.2f);
        }
    }

    private void UpdateTexture(Texture2D texture)
    {
        dissolveShader.ChangeTexture(texture);
        if (Duplicates > 1)
        {
            duplicatesText.text = ArabicToRoman(Duplicates);
            duplicatesText.gameObject.SetActive(true);
        }
        else
        {
            duplicatesText.gameObject.SetActive(false);
        }
    }

    private void UpdateCardTag()
    {
        transform.tag = State is CardData.CardState.NonPickable 
            or CardData.CardState.Placed or CardData.CardState.EnemyHand
            ? "NonPickableCard"
            : "Card";
    }

    private int GenerateUniqueID()
    {
        return ++_globalCardId;
    }

    private bool DetermineTokenType(float token)
    {
        if (IsOperator(token))
        {
            TokenType = CardData.TokenType.Symbol;
        }
        else if (!IsFractional(token))
        {
            TokenType = Mathf.Abs(token) < 10 ? CardData.TokenType.SingleDigit : CardData.TokenType.ManyDigits;
        }
        else
        {
            TokenType = CardData.TokenType.Float;
        }

        return true;
    }
    
    private void DetermineTokenWeight(float token)
    {
        if (IsOperator(token))
        {
            TokenWeight = OperatorToWeight(token);
        }
        else
        {
            TokenWeight = token == 0 ? 7 : Mathf.Abs((int)token);
        }
    }

    private static bool IsFractional(float number) => number % 1 != 0;

    private static bool IsOperator(float token) => token is >= 0.05f and <= 0.08f;

    
    private static string OperatorToString(float token)
    {
        return token switch
        {
            0.05f => "+",
            0.06f => "-",
            0.07f => "\u00d7",
            0.08f => "\u00f7",
            _ => throw new ArgumentOutOfRangeException(nameof(token), token, null)
        };
    }

    private static int OperatorToWeight(float token)
    {
        return token switch
        {
            0.05f => 6,
            0.06f => 4,
            0.07f => 8,
            0.08f => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(token), token, null)
        };
    }
    
    private static string ArabicToRoman(int arabic)
    {
        return arabic switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            4 => "IV",
            5 => "V",
            6 => "VI",
            7 => "VII",
            _ => throw new ArgumentOutOfRangeException(nameof(arabic), arabic, null)
        };
    }
}