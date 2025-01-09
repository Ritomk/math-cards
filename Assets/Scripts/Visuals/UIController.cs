using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private SoUIEvents soUIEvents;
    
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI currentTurnText;

    private int sliderCoroutineId = -1;
    
    private void OnEnable()
    {
        soGameStateEvents.OnGameStateChange += HandleGameStateChange;
        soUIEvents.OnStartSlider += HandleStartSlider;
        soUIEvents.OnEndSlider += HandleEndSlider;
    }

    private void OnDisable()
    {
        soGameStateEvents.OnGameStateChange -= HandleGameStateChange;
        soUIEvents.OnStartSlider -= HandleStartSlider;
        soUIEvents.OnEndSlider -= HandleEndSlider;
    }

    private void HandleGameStateChange(GameStateEnum newstate)
    {
        switch (newstate)
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
}