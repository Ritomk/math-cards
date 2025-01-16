using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
