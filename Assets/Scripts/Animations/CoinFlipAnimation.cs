using System;
using System.Collections;
using UnityEngine;

public class CoinFlipAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    
    [Header("Animation Parameters")]
    [SerializeField] private float flipHeight = 2.0f;
    [SerializeField] private float flipDuration = 1.0f;
    [SerializeField] private int flipAmount = 3;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private bool _isFlipping = false;
    
    void Start()
    {
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (soAnimationEvents != null)
        {
            soAnimationEvents.OnCoinFlipAnimation += StartCoinFlip;
        }
    }

    private void OnDisable()
    {
        if (soAnimationEvents != null)
        {
            soAnimationEvents.OnCoinFlipAnimation -= StartCoinFlip;
        }
    }

    private void StartCoinFlip()
    {
        if (!_isFlipping)
        {
            CoroutineHelper.Start(FlipCoin());
        }
    }

    private IEnumerator FlipCoin()
    {
        _isFlipping = true;

        float elapsedTime = 0f;
        float totalRotation = 360f * flipAmount;
        

        while (elapsedTime < flipDuration)
        {
            float t = elapsedTime / flipDuration;
            
            float height = 4 * flipHeight * t * (1 - t);
            transform.position = _originalPosition + Vector3.up * height;
            
            float rotationAngle = totalRotation * t;
            transform.rotation = _originalRotation * Quaternion.Euler(rotationAngle, 0f, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = _originalPosition;
        transform.rotation = _originalRotation * Quaternion.Euler(totalRotation, 0f, 0f);

        _isFlipping = false;
    }
}
