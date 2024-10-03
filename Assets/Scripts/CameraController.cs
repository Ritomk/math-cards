using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    
    [Space]
    [SerializeField] private Transform lookAtGameObject;
    
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float returnSpeed = 2f;

    private Quaternion _targetRotation;
    private bool _resetCamera = true;
    private bool _isLookingAtTarget = false;

    private void Start()
    {
        if (lookAtGameObject == null)
        {
            Debug.LogError($"{nameof(CameraController)} has not been set properly. {nameof(lookAtGameObject)} is null. Disabling script.", gameObject);
            this.enabled = false;
        }

        if (inputEvents == null)
        {
            Debug.LogError($"{nameof(CameraController)} has not been set properly. {nameof(inputEvents)} is null. Disabling script.", gameObject);
            this.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnLookAround += HandleMouseMove;
            inputEvents.OnCameraReset += HandleCameraReset;
        }

    }

    private void OnDisable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnLookAround -= HandleMouseMove;
            inputEvents.OnCameraReset -= HandleCameraReset;
        }
    }

    private void HandleMouseMove(Vector2 input)
    {
        if (!_resetCamera)
        {
            Vector3 rotate = new Vector3(-input.y * rotationSpeed, input.x * rotationSpeed, 0);
            transform.eulerAngles += rotate;
            
            _isLookingAtTarget = false;
        }
    }

    private void HandleCameraReset(bool reset)
    {
        _resetCamera = reset;
    }

    private void Update()
    {
        if (!_resetCamera || _isLookingAtTarget) return;
        
        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.1f)
        {
            _isLookingAtTarget = true;
            return;
        }
        
        // Kamera wraca do pozycji
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

        _targetRotation = Quaternion.LookRotation(lookAtGameObject.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, returnSpeed * Time.deltaTime);
    }
}
