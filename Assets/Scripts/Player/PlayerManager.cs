using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    
    private void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDestroy()
    {
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerIndex = playerInput.playerIndex;
        Debug.Log($"Player {playerInput.playerIndex} joined with device: {playerInput.devices[0].displayName}");

        AssignPlayerRole(playerInput, playerIndex);
    }

    private void AssignPlayerRole(PlayerInput playerInput, int playerIndex)
    {
        if (playerIndex == 0)
        {
            PlayerInputManager.instance.playerPrefab = player1Prefab;
            Debug.Log("Assigning Player 1 role...");
        }
        else if (playerIndex == 1)
        {
            PlayerInputManager.instance.playerPrefab = player2Prefab;
            Debug.Log("Assigning Player 2 role...");
        }
        else
        {
            Debug.Log($"Assigning default role for Player {playerIndex}...");
        }
    }
}