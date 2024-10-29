using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    
    [Space]
    [SerializeField] private Transform lookAtGameObject;
    
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float returnSpeed = 2f;

    private Quaternion _targetRotation;
    private bool _isResetting = false;
    private int _coroutineId = -1;
    
    private void Start()
    {
        if (lookAtGameObject == null)
        {
            Debug.LogError($"{nameof(CameraController)} has not been set properly. {nameof(lookAtGameObject)} is null. Disabling script.", gameObject);
            this.enabled = false;
            return;
        }

        if (inputEvents == null)
        {
            Debug.LogError($"{nameof(CameraController)} has not been set properly. {nameof(inputEvents)} is null. Disabling script.", gameObject);
            this.enabled = false;
            return;
        }
        
        HandleCameraReset(true);
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
        Vector3 rotate = new Vector3(-input.y * rotationSpeed, input.x * rotationSpeed, 0);
        transform.eulerAngles += rotate;
    }

    private void HandleCameraReset(bool reset)
    {
        _isResetting = reset;
        
        if (_isResetting && _coroutineId == -1)
        {
            Debug.Log("Coroutine Started");
            _targetRotation = Quaternion.LookRotation(lookAtGameObject.position - transform.position);
            _coroutineId = CoroutineHelper.Start(ResetCamera());
        }
    }

    private IEnumerator ResetCamera()
    {
        while (Quaternion.Angle(transform.rotation, _targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, returnSpeed * Time.deltaTime);
            
            yield return new WaitWhile(() => !_isResetting);
        }
        transform.rotation = _targetRotation;
        _coroutineId = -1;
        Debug.Log("Camera Reset Ended");
        yield return null;
    }
}
