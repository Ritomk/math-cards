using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TempUI : MonoBehaviour
{
    [SerializeField] private SoGameStateEvents soGameStateEvents;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI playerStateText;

    private void Update()
    {
        gameStateText.text = soGameStateEvents.RunningGameState.ToString();
        playerStateText.text = soGameStateEvents.RunningPlayerState.ToString();
    }
}
