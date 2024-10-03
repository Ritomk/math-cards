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

    public string Token
    {
        get => textMesh.text;
        set
        {
            textMesh.text = value;
            //Debug.Log(textMesh.text);
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
}
