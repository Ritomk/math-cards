using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CardSelectionController))]
public class CardPickController : MonoBehaviour
{
    [SerializeField] private SoUniversalInputEvents inputEvents;
    [SerializeField] private SoCardEvents soCardEvents;
    [SerializeField] private SoGameStateEvents soGameStateEvents;

    [SerializeField] private Card pickedCard;
    [SerializeField] private Card selectedCard;
    
    [SerializeField] private LayerMask layerMask;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnPickCard += PickCard;
        }

        if (soCardEvents != null)
        {
            soCardEvents.OnCardSelected += SelectCard;
            soCardEvents.OnCardSelectionReset += ResetSelection;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            soCardEvents.RaiseCardSelectionReset();
        }
    }

    private void OnDisable()
    {
        if (inputEvents != null)
        {
            inputEvents.OnPickCard -= PickCard;
        }
        
        if (soCardEvents != null)
        {
            soCardEvents.OnCardSelected -= SelectCard;
            soCardEvents.OnCardSelectionReset -= ResetSelection;
        }
    }

    private void PickCard(bool isPicking)
    {
        if (isPicking)
        {
            if (selectedCard != null && pickedCard == null)
            {
                PlayerStateEnum currentState = soGameStateEvents.CurrentPlayerState;

                switch (currentState)
                {
                    case PlayerStateEnum.PlayerTurnIdle:
                        if (selectedCard.ContainerType == CardContainerType.Hand)
                        {
                            PerformCardSelection(selectedCard);
                        }
                        break;
                    case PlayerStateEnum.CardPlaced:
                        if (selectedCard.ContainerType is CardContainerType.AttackTable
                            or CardContainerType.DefenceTable or CardContainerType.Merger)
                        {
                            PerformCardSelection(selectedCard);
                        }
                        break;
                }
            }
        }
        else
        {
            ReleaseCard();
        }
    }
    
    private void SelectCard(Card card)
    {
        if (pickedCard == null)
        {
            selectedCard = card;
        }
    }

    private void PerformCardSelection(Card card)
    {
        pickedCard = card;
        pickedCard.State = CardData.CardState.Picked;
        soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.CardPicked);
        Debug.Log($"Card picked: {pickedCard.name}");
    }
    
    private void ReleaseCard()
    {
        if (pickedCard == null) return;
        
        Debug.Log($"Card released: {pickedCard.name}");
        pickedCard.State = CardData.CardState.Normal;

        if (IsCardOverTable(out CardContainerBase targetContainer) &&
            targetContainer is TableContainer or MergerContainer)
        {
            if (soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerType, targetContainer.ContainerType))
            {
                soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.CardPlaced);
            }
            else
            {
                soGameStateEvents.RaiseOnRevertPlayerState();
            }

            Debug.Log($"Card over table: {targetContainer.name}");

        }
        else
        {
            HandleCardReturnToHand();
        }
        pickedCard = null;
        selectedCard = null;
    }

    private bool IsCardOverTable(out CardContainerBase container)
    {
        container = null;
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            container = hit.transform.GetComponentInParent<CardContainerBase>();
            return container != null;
        }
        return false;
    }

    private void HandleCardReturnToHand()
    {
        soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
        soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerType, CardContainerType.Hand);
        Debug.Log("Card was not over any valid table");
    }

    private void ResetSelection()
    {
        if (pickedCard != null)
        {
            pickedCard = null;
        }
    }
}
