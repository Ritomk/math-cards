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
        if (!isPicking)
        {
            ReleaseCard();
            return;
        }
        
        if (selectedCard != null && pickedCard == null)
        {
            PlayerStateEnum currentState = soGameStateEvents.CurrentPlayerState;

            switch (currentState)
            {
                case PlayerStateEnum.PlayerTurnIdle:
                case PlayerStateEnum.CardPlacedMerger:
                case PlayerStateEnum.CardPlacedTable:
                    if (selectedCard.ContainerKey.ContainerType is CardContainerType.Hand
                        or CardContainerType.AttackTable
                        or CardContainerType.DefenceTable
                        or CardContainerType.Merger)
                    {
                        PerformCardSelection(selectedCard);
                    }
                    break;
                case PlayerStateEnum.AllCardsPlaced:
                    if (selectedCard.ContainerKey.ContainerType is CardContainerType.AttackTable
                        or CardContainerType.DefenceTable
                        or CardContainerType.Merger)
                    {
                        PerformCardSelection(selectedCard);
                    }
                    break;
            }
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
        if (pickedCard == null)
        {
            return;
        }
        
        Debug.Log($"Card released: {pickedCard.name}");
        pickedCard.State = CardData.CardState.Normal;
        
        var previousState = soGameStateEvents.PreviousPlayerState;

        if (IsCardOverContainer(out CardContainerBase targetContainer))
        {
            var changeState = CanCardMoveToTarget(previousState, targetContainer);
            
            if (changeState is not PlayerStateEnum.Default)
            {
                AttemptCardMove(changeState, previousState, targetContainer);
            }
            else
            {
                soGameStateEvents.RaiseOnRevertPlayerState();
                Debug.Log("Invalid container for the current state.");
            }
        }
        else
        {
            HandleCardReturnToHand(previousState);
        }
        pickedCard = null;
        selectedCard = null;
    }

    private bool IsCardOverContainer(out CardContainerBase container)
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

    private PlayerStateEnum CanCardMoveToTarget(PlayerStateEnum state, CardContainerBase targetContainer)
    {
        switch (targetContainer)
        {
            case MergerContainer when state == PlayerStateEnum.PlayerTurnIdle:
                return PlayerStateEnum.CardPlacedMerger;

            case TableContainer when state == PlayerStateEnum.PlayerTurnIdle:
                return PlayerStateEnum.CardPlacedTable;
            
            case MergerContainer when state == PlayerStateEnum.CardPlacedTable:
                return pickedCard.ContainerKey.ContainerType is CardContainerType.AttackTable
                    or CardContainerType.DefenceTable
                    ? PlayerStateEnum.CardPlacedMerger
                    : PlayerStateEnum.AllCardsPlaced;
            
            case TableContainer when state == PlayerStateEnum.CardPlacedTable:
                return pickedCard.ContainerKey.ContainerType is CardContainerType.AttackTable
                    or CardContainerType.DefenceTable
                    ? PlayerStateEnum.CardPlacedTable
                    : PlayerStateEnum.Default;
            
            case MergerContainer when state == PlayerStateEnum.CardPlacedMerger:
                return pickedCard.ContainerKey.ContainerType is CardContainerType.Merger
                    ? PlayerStateEnum.CardPlacedMerger
                    : PlayerStateEnum.Default;
                
            case TableContainer when state == PlayerStateEnum.CardPlacedMerger:
                return pickedCard.ContainerKey.ContainerType is CardContainerType.Merger
                    ? PlayerStateEnum.CardPlacedTable
                    : PlayerStateEnum.AllCardsPlaced;
            
            case TableContainer when state == PlayerStateEnum.AllCardsPlaced:
                return pickedCard.ContainerKey.ContainerType is CardContainerType.AttackTable
            or CardContainerType.DefenceTable
            ? PlayerStateEnum.AllCardsPlaced
            : PlayerStateEnum.Default;
        }

        return PlayerStateEnum.Default;
    }

    private void AttemptCardMove(PlayerStateEnum changeState, PlayerStateEnum state, CardContainerBase targetContainer)
    {
        if (soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerKey, targetContainer.SelfContainerKey))
        {
            soGameStateEvents.RaiseOnPlayerStateChange(changeState);
        }
        else
        {
            RevertState(state);
        }
    }
    
    private void HandleCardReturnToHand(PlayerStateEnum state)
    {
        RevertState(state);
        
        var toKey = new ContainerKey(OwnerType.Player, CardContainerType.Hand);
        soCardEvents.RaiseCardMove(pickedCard, pickedCard.ContainerKey, toKey);
        Debug.Log("Card was not over any valid table");
    }

    private void RevertState(PlayerStateEnum state)
    {
        switch (pickedCard.ContainerKey.ContainerType)
        {
            case CardContainerType.Merger:
                soGameStateEvents.RaiseOnPlayerStateChange(state == PlayerStateEnum.AllCardsPlaced
                    ? PlayerStateEnum.CardPlacedTable
                    : PlayerStateEnum.PlayerTurnIdle);
                break;
        
            case CardContainerType.AttackTable:
            case CardContainerType.DefenceTable:
                soGameStateEvents.RaiseOnPlayerStateChange(state == PlayerStateEnum.AllCardsPlaced
                    ? PlayerStateEnum.CardPlacedMerger
                    : PlayerStateEnum.PlayerTurnIdle);
                break;
            
            default:
                soGameStateEvents.RaiseOnRevertPlayerState();
                break;
        }
    }

    private void ResetSelection()
    {
        if (pickedCard != null)
        {
            pickedCard = null;
        }
    }
}
