using UnityEngine;
using System.Collections;

public class ChestAnimation : MonoBehaviour
{
    [SerializeField] private MergerContainer mergerContainer;
    [SerializeField] private SoAnimationEvents soAnimationEvents;
    [SerializeField] private float animationDuration = 1.0f;

    private readonly Quaternion _openRotation = Quaternion.Euler(45, 90, -90);
    private readonly Quaternion _closedRotation = Quaternion.Euler(-90, 90, -90);

    private bool _isOpen = true;
    public bool IsMoving { get; private set; } = false;

    public void ToggleLead(bool isOpen)
    {
        if (isOpen != _isOpen && !IsMoving)
        {
            var rotation = _isOpen ? _closedRotation : _openRotation;
            _isOpen = !_isOpen;
            CoroutineHelper.Start(RotateContainer(rotation));
        }
    }

    private IEnumerator RotateContainer(Quaternion targetRotation)
    {
        IsMoving = true;
        
        Quaternion initialRotation = transform.localRotation;
        float elapsedTime = 0;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }
        
        transform.localRotation = targetRotation;
        
        IsMoving = false;
    }
}