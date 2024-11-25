using System;
using System.Collections;
using UnityEngine;

public class HighlightEffect : MonoBehaviour
{
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    private static readonly int OutlineColorID = Shader.PropertyToID("_Outline_Color");
    private static readonly int RainColorID = Shader.PropertyToID("_Rain_Color");
    private static readonly int RainSpeedValueID = Shader.PropertyToID("_Rain_Speed");
    private static readonly int AccumulatedTimeID = Shader.PropertyToID("_Accumulated_Time");
    
    private Material _material;
    [SerializeField] private float originalRainSpeed = 0.1f;
    // private float _rainSpeed;
    // private float _accumulatedTime = 0f;
    
    private int _outlineCoroutineId = -1;
    private int _highlightCoroutineId = -1;
    private int _addCardCoroutineId = -1;

    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        // _rainSpeed = originalRainSpeed;
    }

    private void OnEnable() => soGameStateEvents.OnPauseGame += HandleChangeRainSpeed;

    private void OnDisable() => soGameStateEvents.OnPauseGame -= HandleChangeRainSpeed;

    //TODO: Static Update for All copies
    // private void Update()
    // {
    //     _accumulatedTime += Time.deltaTime * _rainSpeed;
    //     Debug.Log(_accumulatedTime);
    //     _material.SetFloat(AccumulatedTimeID, _accumulatedTime);
    // }

    private void HandleChangeRainSpeed(bool paused)
    {
        var rainSpeed = paused ? 0 : originalRainSpeed;
        // _rainSpeed = rainSpeed;
        ChangeRainSpeedValue(rainSpeed);
    }

    public void ChangeOutlineColor(Color color)
    {
        _material.SetColor(OutlineColorID, color);
    }

    public void ChangeHighlightColor(Color color)
    {
        _material.SetColor(RainColorID, color);
    }

    public void ChangeRainSpeedValue(float value)
    {
        _material.SetFloat(RainSpeedValueID, value);
    }

    public void SmoothChangeOutlineColor(Color targetColor, float duration)
    {
        if (_outlineCoroutineId != -1)
        {
            CoroutineHelper.Stop(_outlineCoroutineId);
            _outlineCoroutineId = -1;
        }
        _outlineCoroutineId = CoroutineHelper.Start(SmoothChangeOutlineColorCoroutine(targetColor, duration));
    }

    public void SmoothChangeHighlightColor(Color targetColor, float duration)
    {
        if (_highlightCoroutineId != -1)
        {
            CoroutineHelper.Stop(_highlightCoroutineId);
            _highlightCoroutineId = -1;
        }
        _highlightCoroutineId = CoroutineHelper.Start(SmoothChangeHighlightColorCoroutine(targetColor, duration));
    }

    public void AddCardToHand(Color baseOutlineColor, Color baseHighlightColor, Color changeOutlineColor,
        Color changeHighlightColor, float duration)
    {
        if(_addCardCoroutineId != -1) return;

        _addCardCoroutineId = CoroutineHelper.Start(AddCardToHandCoroutine(baseOutlineColor, baseHighlightColor,
            changeOutlineColor, changeHighlightColor, duration));
    }

    private IEnumerator AddCardToHandCoroutine(Color baseOutlineColor, Color baseHighlightColor, Color changeOutlineColor,
        Color changeHighlightColor, float duration)
    {
        float halfDuration = duration / 2f;

        if (_outlineCoroutineId != -1)
        {
            CoroutineHelper.Stop(_outlineCoroutineId);
            _outlineCoroutineId = -1;
        }
        
        if (_highlightCoroutineId != -1)
        {
            CoroutineHelper.Stop(_highlightCoroutineId);
            _highlightCoroutineId = -1;
        }
        
        CoroutineHelper.Start(SmoothChangeOutlineColorCoroutine(changeHighlightColor, halfDuration));
        CoroutineHelper.Start(SmoothChangeHighlightColorCoroutine(changeHighlightColor, halfDuration));
        
        yield return new WaitForSecondsPauseable(halfDuration);
        
        CoroutineHelper.Start(SmoothChangeOutlineColorCoroutine(baseOutlineColor, halfDuration));
        CoroutineHelper.Start(SmoothChangeHighlightColorCoroutine(baseHighlightColor, halfDuration));
        
        yield return new WaitForSecondsPauseable(halfDuration);
        
        _addCardCoroutineId = -1;
    }

    private IEnumerator SmoothChangeOutlineColorCoroutine(Color targetColor, float duration)
    {
        Color initialColor = _material.GetColor(OutlineColorID);
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(initialColor, targetColor, time / duration);
            _material.SetColor(OutlineColorID, newColor);
            yield return null;
        }

        _material.SetColor(OutlineColorID, targetColor);
    }

    private IEnumerator SmoothChangeHighlightColorCoroutine(Color targetColor, float duration)
    {
        Color initialColor = _material.GetColor(RainColorID);
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            Color newColor = Color.Lerp(initialColor, targetColor, time / duration);
            _material.SetColor(RainColorID, newColor);
            yield return null;
        }

        _material.SetColor(RainColorID, targetColor);
    }
}