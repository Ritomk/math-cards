using UnityEngine;

public class TimerAnimation : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Range(0f, 1f)] [SerializeField] private float length = 1.0f;

    public float Length
    {
        get => length;
        set
        {
            value = Mathf.Clamp01(value);
            length = value;
            UpdateLength();
        }
    }

    private Vector3 _originalScale;
    private Vector3 _direction;
    private float _initialLength;

    private void Start()
    {
        _originalScale = transform.localScale;
        _direction = (endPoint.position - startPoint.position).normalized;
        _initialLength = Vector3.Distance(startPoint.position, endPoint.position);

        transform.position = startPoint.position;
        transform.rotation = Quaternion.LookRotation(_direction);
        transform.localScale = new Vector3(_originalScale.x, _originalScale.y, _initialLength);
        
        UpdateLength();
    }

    private void UpdateLength()
    {
        float currentLength = _initialLength * length;
        transform.localScale = new Vector3(_originalScale.x, _originalScale.y, currentLength);
        transform.position = startPoint.position + _direction * (currentLength / 2.0f);
    }
}
