using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public enum CardState
    {
        Normal,
        Highlighted,
        Picked,
        Placed,
        NonPickable
    }
    
    [Serializable]
    public struct StateColorPair
    {
        public CardState state;
        public Color color;
    }
    
    [SerializeField] private StateColorPair[] stateColors;

    private Dictionary<CardState, Color> _stateColorMap;

    private void OnEnable()
    {
        InitializeStateColorMap();
    }

    private void InitializeStateColorMap()
    {
        _stateColorMap = new Dictionary<CardState, Color>();
        foreach (var pair in stateColors)
        {
            _stateColorMap[pair.state] = pair.color;
        }
    }

    public Color GetColorForState(CardState state)
    {
        return _stateColorMap.TryGetValue(state, out var color) ? color : Color.white;
    }
}
