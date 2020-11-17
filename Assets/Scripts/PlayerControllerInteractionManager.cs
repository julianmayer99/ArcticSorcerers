using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerControllerInteractionManager : MonoBehaviour
{
    [Serializable]
    public class PlayerControllerDualEvent : UnityEvent<PlayerController, PlayerController> { };

    /// <summary>
    /// <Killer, Victim>
    /// </summary>
    public PlayerControllerDualEvent OnPlayerHitOtherPlayer;
    public PlayerControllerDualEvent OnPlayerKilledOtherPlayer;
    public static PlayerControllerInteractionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
