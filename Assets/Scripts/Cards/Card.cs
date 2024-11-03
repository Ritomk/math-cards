using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Serialization;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private Renderer cardRenderer;
    [SerializeField] private DissolveEffect dissolveShader;
    [SerializeField] private HighlightEffect highlightShader;
    
    [field: SerializeField]
    public CardContainerType ContainerType { get; set; }
    
    private CardData.CardState _currentState = CardData.CardState.Normal;
    
    private static int _globalCardId = -1;
    
    public int CardId { get; private set; }
    
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
                textMesh.text = "Error"; 
                Debug.LogError($"Invalid token type: {value}", gameObject);
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
    }
    
    public void DestroyCard() => StartCoroutine(DissolveAndDestroy());

    private IEnumerator DissolveAndDestroy()
    {
        yield return CoroutineHelper.StartAndWait(dissolveShader.StartDissolve());
        Destroy(gameObject);
        
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
        else if (token is "+" or "-" or "\u00d7" or "\u00f7")
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
