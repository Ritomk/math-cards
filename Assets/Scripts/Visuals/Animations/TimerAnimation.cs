using System;
using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    
    [SerializeField] private Transform timerTransform;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Range(0f, 1f)] [SerializeField] private float length = 1.0f;

    
    private Vector3 _originalScale;
    private Vector3 _direction;
    private float _initialLength;
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
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Start and end points are required.");
            enabled = false;
            return;
        }
        
        _originalScale = timerTransform.localScale;
        _direction = (endPoint.position - startPoint.position).normalized;
        _initialLength = Vector3.Distance(startPoint.position, endPoint.position);

        timerTransform.position = startPoint.position;
        timerTransform.rotation = Quaternion.LookRotation(_direction);
        timerTransform.localScale = new Vector3(_originalScale.x, _originalScale.y, _initialLength);
        
        timerTransform.gameObject.SetActive(_isTransformVisible);
    }

    private void UpdateLength(float t)
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
        length = Mathf.Clamp01(t);
        Debug.Log($"Update length: {length}, t: {t}");
        
        float currentLength = _initialLength * length;
        timerTransform.localScale = new Vector3(_originalScale.x, _originalScale.y, currentLength);
        timerTransform.position = startPoint.position + _direction * (currentLength / 2.0f);
    }
}
