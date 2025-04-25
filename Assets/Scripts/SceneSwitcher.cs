using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static int gameMode;
    
    private void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    } 
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetGameMode(int player)
    {
        gameMode = player;
    }
}
