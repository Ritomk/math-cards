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
                            or CardContainerType.DefenceTable)
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
        if (pickedCard != null)
        {
            Debug.Log($"Card released: {pickedCard.name}");
            pickedCard.State = CardData.CardState.Normal;

            if (IsCardOverTable(out TableContainer targetTable))
            {
                if (soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerType, targetTable.ContainerType))
                {
                    soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.CardPlaced);
                }
                else
                {
                    soGameStateEvents.RaiseOnRevertPlayerState();
                }
                Debug.Log($"Card over table: {targetTable.name}");
            }
            else
            {
                soGameStateEvents.RaiseOnPlayerStateChange(PlayerStateEnum.PlayerTurnIdle);
                soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerType, CardContainerType.Hand);
                Debug.Log("Card was not over any table");
            }
            pickedCard = null;
            selectedCard = null;
        }
    }

    private bool IsCardOverTable(out TableContainer tableContainer)
    {
        tableContainer = null;
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            tableContainer = hit.transform.GetComponentInParent<TableContainer>();
            return tableContainer != null;
        }
        return false;
    }

    private void ResetSelection()
    {
        if (pickedCard != null)
        {
            pickedCard = null;
        }
    }
}
