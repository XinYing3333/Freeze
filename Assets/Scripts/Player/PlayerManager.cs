using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
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
        Debug.Log($"Player {playerIndex} has joined the game!");

        // 可根据 playerIndex 分配自定义角色或行为
        AssignPlayerRole(playerInput, playerIndex);
    }

    private void AssignPlayerRole(PlayerInput playerInput, int playerIndex)
    {
        if (playerIndex == 0)
        {
            Debug.Log("Assigning Player 1 role...");
        }
        else if (playerIndex == 1)
        {
            Debug.Log("Assigning Player 2 role...");
        }
        else
        {
            Debug.Log($"Assigning default role for Player {playerIndex}...");
        }
    }
}