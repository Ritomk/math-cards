using System;
using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Range(0f, 1f)] [SerializeField] private float length = 1.0f;

    private void OnEnable()
    {
        soAnimationEvents.TimerAnimation += UpdateLength;
    }

    private void OnDestroy()
    {
        soAnimationEvents.TimerAnimation -= UpdateLength;
    }

    private Vector3 _originalScale;
    private Vector3 _direction;
    private float _initialLength;

    private void UpdateLength(float t)
    {
        length = Mathf.Clamp01(t);
        UpdateLength();
    }

    private void Awake()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Start and end points are required.");
            enabled = false;
            return;
        }
        
        _originalScale = transform.localScale;
        _direction = (endPoint.position - startPoint.position).normalized;
        _initialLength = Vector3.Distance(startPoint.position, endPoint.position);

        transform.position = startPoint.position;
        transform.rotation = Quaternion.LookRotation(_direction);
        transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _initialLength);
        
        
        Debug.Log("Dupa");
        UpdateLength();
    }

    private void UpdateLength()
    {
        float currentLength = _initialLength * length;
        transform.localScale = new Vector3(_originalScale.x, _originalScale.y, currentLength);
        transform.position = startPoint.position + _direction * (currentLength / 2.0f);
    }
}
