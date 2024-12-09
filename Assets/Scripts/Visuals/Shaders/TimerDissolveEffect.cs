using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDissolveEffect : MonoBehaviour
{
    private static readonly int BurnPercent = Shader.PropertyToID("_BurnPercent");
    private Material _material;

    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private Transform timerTransform;
    
    private bool _isTransformVisible = false;

    private void OnEnable()
    {
        soAnimationEvents.OnTimerAnimation += UpdateLength;
    }

    private void OnDisable()
    {
        soAnimationEvents.OnTimerAnimation -= UpdateLength;
    }
    
    private void Awake()
    {
        _material = timerTransform.GetComponent<MeshRenderer>().material;
        Debug.Log(_material);
    }
    
    public void UpdateLength(float t)
    {
        if (t <= 0)
        {
            _isTransformVisible = false;
            timerTransform.gameObject.SetActive(_isTransformVisible);
            return;
        }
        else if (!_isTransformVisible)
        {
            _isTransformVisible = true;
            timerTransform.gameObject.SetActive(_isTransformVisible);
        }
        var percent = Mathf.Clamp01(t);
        _material.SetFloat(BurnPercent, percent);
    }
}
