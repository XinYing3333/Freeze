using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        // Initialize game settings or states here
    }

    private void Update()
    {
        // Global game logic can be managed here
    }

    // Example of a method to manage game states
    public void StartGame()
    {
        // Logic to start the game
    }

    public void EndGame()
    {
        // Logic to end the game
    }
}