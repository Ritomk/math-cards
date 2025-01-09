using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public enum CardState
    {
        Normal,
        Highlighted,
        Picked,
        Placed,
        EnemyHand,
        NonPickable
    }

    public enum TokenType
    {
        SingleDigit,
        DoubleDigit,
        Symbol,
        IllegalToken
    }
    
    [Serializable]
    public struct StateColorSet
    {
        public CardState state;
        public Color color;
        public bool hasHighlight;
        [ShowIf("hasHighlight")][ColorUsage(true, true)]
        public Color outlineColor;
        [ShowIf("hasHighlight")][ColorUsage(true, true)]
        public Color highlightColor;
        
        public StateColorSet(CardState state, Color baseColor, bool hasHighlight = false, Color outlineColor = default, Color highlightColor = default)
        {
            this.state = state;
            this.color = baseColor;
            this.hasHighlight = hasHighlight;
            this.outlineColor = outlineColor;
            this.highlightColor = highlightColor;
        }
    }
    
    [SerializeField] private StateColorSet[] stateColorSets;

    private Dictionary<CardState, StateColorSet> _stateColorMap;

    //Todo: Rewrite for OnValidate
    private void OnEnable()
    {
        InitializeStateColorMap();
    }

    private void InitializeStateColorMap()
    {
        _stateColorMap = new Dictionary<CardState, StateColorSet>();
        foreach (var colorSet  in stateColorSets)
        {
            _stateColorMap[colorSet.state] = colorSet;
        }
    }

    public StateColorSet GetColorSetForState(CardState state)
    {
        return _stateColorMap.GetValueOrDefault(state);
    }
}
