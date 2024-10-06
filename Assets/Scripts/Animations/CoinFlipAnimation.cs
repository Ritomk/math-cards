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
            soAnimationEvents.CoinFlipAnimation += StartCoinFlip;
        }
    }

    private void OnDisable()
    {
        if (soAnimationEvents != null)
        {
            soAnimationEvents.CoinFlipAnimation -= StartCoinFlip;
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
        Vector3 targetPosition = _originalPosition + Vector3.up * flipHeight;
        float totalRotation = 360f * flipAmount;
        
        float rotationPerSecond = totalRotation / flipDuration;

        while (elapsedTime < flipDuration)
        {
            float t = elapsedTime / flipDuration;
            
            float positionT;
            if (t < 0.5f)
            {
                positionT = Mathf.SmoothStep(0f, 1f, t * 2f);
                transform.position = Vector3.Lerp(_originalPosition, targetPosition, positionT);
            }
            else
            {
                positionT = Mathf.SmoothStep(1f, 0f, (t - 0.5f) * 2f);
                transform.position = Vector3.Lerp(_originalPosition, targetPosition, positionT);
            }
            
            float rotationThisFrame = rotationPerSecond * Time.deltaTime;
            transform.Rotate(Vector3.right, rotationThisFrame, Space.World);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = _originalRotation * Quaternion.Euler(totalRotation, 0f, 0f);

        _isFlipping = false;
    }
}
