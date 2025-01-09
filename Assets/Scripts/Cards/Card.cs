using System;
using UnityEngine;
using TMPro;
using System.Collections;

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

    public CardData.TokenType TokenType { get; private set; } = CardData.TokenType.IllegalToken;

    private int _token;
    
    public int Token
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

    public void Initialize(int token = 0, bool isTokenVisible = true,
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
        tokenText.text = IsOperator(_token) ? OperatorToString(_token) : _token.ToString();
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
        transform.tag = State is CardData.CardState.NonPickable or CardData.CardState.Placed
            ? "NonPickableCard"
            : "Card";
    }

    private int GenerateUniqueID()
    {
        return ++_globalCardId;
    }

    private bool DetermineTokenType(int token)
    {
        if (!IsOperator(token))
        {
            if (token is >= -99 and <= 99)
            {
                TokenType = token / 10 == 0 ? CardData.TokenType.SingleDigit : CardData.TokenType.DoubleDigit;
            }
            else
            {
                TokenType = CardData.TokenType.IllegalToken;
                return false;
            }
        }
        else
        {
            TokenType = CardData.TokenType.Symbol;
        }
        return true;
    }
    
    
    private void DetermineTokenWeight(int token)
    {
        if (IsOperator(token))
        {
            TokenWeight = OperatorToWeight(token);
        }
        else
        {
            TokenWeight = token == 0 ? 7 : Mathf.Abs(token);
        }
    }

    private static bool IsOperator(int token)
    {
        return token is >= 101 and <= 104;
    }

    private static string OperatorToString(int token)
    {
        return token switch
        {
            101 => "+",
            102 => "-",
            103 => "\u00d7",
            104 => "\u00f7",
            _ => throw new ArgumentOutOfRangeException(nameof(token), token, null)
        };
    }

    private static int OperatorToWeight(int token)
    {
        return token switch
        {
            101 => 6,
            102 => 4,
            103 => 8,
            104 => 8,
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
