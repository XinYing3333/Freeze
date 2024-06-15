using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI State")]
    public float scoreCount;
    public int enemyKill;
    public Text scoreText;
    public Animator uiAnim;
    public Text finalScoreText;
    public Text enemyKillText;
    public Text highestScoreText;
    
    public bool isOver;
    private GameObject _gameOverMenu;
    private float _highestScore;

    
    private void Start()
    {
        _gameOverMenu = GameObject.Find("GameOver");
        _gameOverMenu.SetActive(false);
        
        GameObject score = GameObject.Find("Score");
        scoreText = score.GetComponent<Text>();
        uiAnim = score.GetComponent<Animator>();

        isOver = false;
        scoreCount = 0f;
        enemyKill = 0;
        
        _highestScore = PlayerPrefs.GetFloat("HighestScore", 0f);
        highestScoreText.text = "Highest Score: " + _highestScore.ToString("");
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
        
        if (scoreCount > _highestScore)
        {
            _highestScore = scoreCount;
            PlayerPrefs.SetFloat("HighestScore", _highestScore);
            PlayerPrefs.Save();
            highestScoreText.text = "Highest Score: " + _highestScore.ToString("");
        }
    }

    public void EndGame()
    {
        if (isOver)
        {
            _gameOverMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        isOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/GameScene");
    }
}