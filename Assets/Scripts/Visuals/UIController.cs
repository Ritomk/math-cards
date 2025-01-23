using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private SoUIEvents soUIEvents;
    
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private GameObject endGameObject;
    [SerializeField] private TextMeshProUGUI endGameText;
    
    [SerializeField] private List<GameObject> playerCrystals = new List<GameObject>();
    [SerializeField] private List<GameObject> opponentCrystals = new List<GameObject>();

    private int sliderCoroutineId = -1;
    
    private void OnEnable()
    {
        soGameStateEvents.OnGameStateChange += HandleGameStateChange;
        soUIEvents.OnStartSlider += HandleStartSlider;
        soUIEvents.OnEndSlider += HandleEndSlider;
        soUIEvents.OnSetCrystalsAmount += HandleSetCrystalsAmount;
        soUIEvents.OnEndGame += HandleEndGame;
    }

    private void OnDisable()
    {
        soGameStateEvents.OnGameStateChange -= HandleGameStateChange;
        soUIEvents.OnStartSlider -= HandleStartSlider;
        soUIEvents.OnEndSlider -= HandleEndSlider;
        soUIEvents.OnSetCrystalsAmount -= HandleSetCrystalsAmount;
        soUIEvents.OnEndGame += HandleEndGame;
    }

    private void HandleGameStateChange(GameStateEnum newState)
    {
        switch (newState)
        {
            case GameStateEnum.PlayerTurn:
                UpdateTurnText("Your\nTurn", true);
                break;
            case GameStateEnum.OpponentTurn:
                UpdateTurnText("Barman's\nTurn", true);
                break;
            case GameStateEnum.BeginRound:
                UpdateTurnText("Dealing\nThe Cards", true);
                break;
            default:
                UpdateTurnText(string.Empty, false);
                break;
        }
    }

    private void UpdateTurnText(string text, bool isActive)
    {
        Debug.Log("UI: UpdateTurnText");
        currentTurnText.enabled = isActive;
        
        if(isActive)
            currentTurnText.text = text;
    }

    private void HandleStartSlider()
    {
        if (sliderCoroutineId != -1)
        {
            CoroutineHelper.Stop(sliderCoroutineId);
        }

        sliderCoroutineId = CoroutineHelper.Start(AnimateSlider(0, 1, 1.5f));
    }

    private void HandleEndSlider()
    {
        if (sliderCoroutineId != -1)
        {
            CoroutineHelper.Stop(sliderCoroutineId);
            slider.value = slider.minValue;
        }
    }

    private IEnumerator AnimateSlider(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = 1 - Mathf.Pow(1 - t, 3);

            slider.value = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
        slider.value = endValue;
    }

    private void HandleSetCrystalsAmount(int playerAmount, int opponentAmount)
    {
        if (playerAmount > playerCrystals.Count)
        {
            Debug.LogWarning($"Player crystal count {playerAmount} is out of range ({playerCrystals.Count})." +
                             " Clamping to valid range.");
            playerAmount = Mathf.Clamp(playerAmount, 0, playerCrystals.Count);
        }

        if (opponentAmount > opponentCrystals.Count)
        {
            Debug.LogWarning($"Opponent crystal count {opponentAmount} is out of range ({opponentCrystals.Count})." +
                             " Clamping to valid range.");
            opponentAmount = Mathf.Clamp(opponentAmount, 0, opponentCrystals.Count);
        }
        
        for (int i = 0; i < playerAmount; i++)
        {
            playerCrystals[i].SetActive(i < playerAmount);
        }

        for (int i = 0; i < opponentAmount; i++)
        {
            opponentCrystals[i].SetActive(i < opponentAmount);
        }
    }

    private void HandleEndGame(bool playerWon)
    {
        endGameObject.SetActive(true);
        
        if (playerWon)
        {
            endGameText.text = "You\nwon!";
        }
        else
        {
            endGameText.text = "Barman\nwon...";
        }
    }
}