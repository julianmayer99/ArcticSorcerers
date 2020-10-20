using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
    public DynamicMultiTargetCamera dynamicCamera;
    public List<PlayerController> players;

    public PlayerConfiguration RegisterPlayerAndGetConfiguration(PlayerController player)
    {
        // Developoment data
        var config = new PlayerConfiguration
        {
            playerId = (byte)players.Count,
            playerName = "Player " + (players.Count + 1)
        };

        dynamicCamera.targets.Add(player.transform);
        players.Add(player);
        return config;
    }

    public void OnLocalPlayerJoined(PlayerInput playerInput)
    {

    }

    public void OnLocalPlayerLeft(PlayerInput playerInput)
    {

    }

}
