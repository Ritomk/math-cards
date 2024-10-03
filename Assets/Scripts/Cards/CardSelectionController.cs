using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Rendering;

public class CardSelectionController : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    
    private Camera _cameraRef;

    private void Awake()
    {
        _cameraRef = Camera.main;
    }

    private void OnEnable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnMouseMove += DetectCardUnderMouse;
        }
    }

    private void OnDisable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnMouseMove -= DetectCardUnderMouse;
        }

        if (soCardEvents != null)
        {
            soCardEvents.RaiseCardSelectionReset();
            soCardEvents.RaiseCardSelected(null);
        }
    }

    private void Update()
    {
        //Debug.Log(selectedCard != null ? selectedCard.transform.name : null);
    }

    private void DetectCardUnderMouse()
    {
        Ray ray = _cameraRef.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Card") && hit.collider.TryGetComponent<Card>(out Card card))
            { 
                soCardEvents.RaiseCardSelected(card);
                return;
            }
        }
        soCardEvents.RaiseCardSelected(null);
    }
}
