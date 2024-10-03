using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerVisualizer : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Range(0f, 1f)] [SerializeField] private float t = 1.0f;

    private Vector3 _originalScale;

    private void Start()
    {
        float initialLength = Vector3.Distance(startPoint.position, endPoint.position);
        
        _originalScale = transform.localScale;
        transform.localScale = new Vector3(initialLength, _originalScale.y, initialLength);
        
        transform.position = startPoint.position;
    }

    private void Update()
    {
        float currentLength = Vector3.Distance(startPoint.position, endPoint.position) * t;
        
        transform.localScale = new Vector3(_originalScale.x, _originalScale.y, currentLength);
        
        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        transform.position = startPoint.position + direction * (currentLength / 2.0f);
    }
}
