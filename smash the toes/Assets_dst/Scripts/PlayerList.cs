using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerList : MonoBehaviour
{
    public static List<PlayerInput> players = new List<PlayerInput>();

    void Start()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
            PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
        }
        else
        {
            Debug.LogError("PlayerInputManager instance is still null in Start!");
        }
    }

    void OnDestroy()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
            PlayerInputManager.instance.onPlayerLeft -= OnPlayerLeft;
        }
    }

    void OnPlayerJoined(PlayerInput player)
    {
        players.Add(player);
    }

    void OnPlayerLeft(PlayerInput player)
    {
        players.Remove(player);
    }
}
