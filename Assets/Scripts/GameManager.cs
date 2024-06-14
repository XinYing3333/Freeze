using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI State")]
    public float scoreCount;
    public int enemyKill;
    public Text scoreText;
    public Text finalScoreText;
    public Text enemyKillText;

    
    public bool isOver;
    private GameObject _gameOverMenu;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        _gameOverMenu = GameObject.Find("GameOver");
        _gameOverMenu.SetActive(false);
        
        GameObject score = GameObject.Find("Score");
        scoreText = score.GetComponent<Text>();

        isOver = false;
        scoreCount = 0f;
        enemyKill = 0;
    }

    private void Update()
    {
       UpdateScore();
       EndGame();
    }

    public void StartGame()
    {
        
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + scoreCount.ToString("");
        finalScoreText.text = scoreText.text;
        enemyKillText.text = "Enemy Killed: " + enemyKill.ToString("");
    }

    public void EndGame()
    {
        if (isOver)
        {
            _gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
}
}