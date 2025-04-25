using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("===== UI State =====")]
    [Space(10)]
    public float scoreCount;
    public int enemyKill;
    public Animator uiAnim;
    [Space(10)]
    [SerializeField] private GameObject controlMenu;
    [SerializeField] private GameObject gameOverMenu;
    [Space(10)]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private Text enemyKillText;
    [SerializeField] private Text highestScoreText;
    [SerializeField] private Text timerText;

    
    [Header("===== Timer Settings =====")]
    [Space(10)]
    [SerializeField] private float setDefaultTimer = 5f;
    private float _currentTimer;

    
    [Header("===== PlayerMode Settings =====")]
    [Space(10)]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    
    [HideInInspector] public bool isOver;
    private float _highestScore;
    
    
    private void Start()
    {
        SetPlayerMode();
        ResetAllState();
    }

    private void Update()
    {
       UpdateScore();
       StartTimer();
       CheckEndGame();
       if (Input.GetKeyDown(KeyCode.M))
       {
           MainMenu();
       }
    }
    
    private void ResetAllState()
    {
        GameObject score = GameObject.Find("Score");
        scoreText = score.GetComponent<Text>();
        uiAnim = score.GetComponent<Animator>();
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.name == "Player1Prefab")
            {
                player1 = player;
            }
            else if (player.name == "Player2Prefab")
            {
                player2 = player;
            }
        }

        scoreCount = 0f;
        enemyKill = 0;
        _currentTimer = setDefaultTimer;
        
        _highestScore = PlayerPrefs.GetFloat("HighestCoin", 0f);
        highestScoreText.text = "Highest Coin: " + _highestScore.ToString("");
    }

    private void UpdateScore()
    {
        scoreText.text = "Coin: " + scoreCount.ToString("");
        finalScoreText.text = scoreText.text;
        enemyKillText.text = "FinishedOrders: " + enemyKill.ToString("");
        
        if (scoreCount > _highestScore)
        {
            _highestScore = scoreCount;
            PlayerPrefs.SetFloat("HighestCoin", _highestScore);
            PlayerPrefs.Save();
            highestScoreText.text = "Highest Coin: " + _highestScore.ToString("");
        }
    }

    private void StartTimer()
    {
        _currentTimer -= Time.deltaTime;
        timerText.text = "Timer: " + (int)_currentTimer;
        if (_currentTimer <= 0)
        {
            isOver = true;
        }
    }

    private void CheckEndGame()
    {
        if (isOver)
        {
            gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void SetPlayerMode()
    {
        ResetPlayerCount();
        switch (SceneSwitcher.gameMode)
        {
            case 2:
                player2.SetActive(true);
                break;
            //case 3:
                //player3.SetActive(true);
                //break;
        }
    }
    
    private void ResetPlayerCount()
    {
        player2.SetActive(false);
        // If we want to add more player
        // player3.SetActive(false);
    }

    public void RestartGame()
    {
        _currentTimer = setDefaultTimer;
        gameOverMenu.SetActive(false);
        isOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/GameScene");
    }
    
    public void MainMenu()
    {
        isOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void ControlMenu()
    {
        var isActive = controlMenu.activeSelf;
        controlMenu.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;
    }
}