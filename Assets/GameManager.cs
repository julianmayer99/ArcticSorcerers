using Assets.Scripts.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ArcticSorcerersMap map; // TODO: Sollte später gespawnt werden je nach GameSettings
    public DynamicMultiTargetCamera dynamicCamera;
    public GameObject playerPrefab;
    public static GameManager Instance{ get; set; }

    [Header("UI Stuff")]
    [Space]
    public PlusTextPopup popUpPlusTextPreFab;

    private void Awake()
    {
        Instance = this;

        if (PlayerConfigurationManager.Instance == null)
        {
            GoToJoinRoom();
            return;
        }
    }

    private void Start()
    {
        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            DynamicMultiTargetCamera.instance.targets.Add(player.transform);
            player.RespawnUI();
        }
    }

    private void OnEnable()
    {
        PlayerControllerInteractionManager.Instance.OnPlayerHitOtherPlayer.AddListener(OnPlayerHitOtherPlayer_Basic_GamemodeIndependent);
        PlayerControllerInteractionManager.Instance.OnPlayerKilledOtherPlayer.AddListener(OnPlayerKilledOtherPlayer_Basic_GamemodeIndependent);

        PlayerControllerInteractionManager.Instance.OnPlayerKilledOtherPlayer.AddListener(GameSettings.gameMode.OnPlayerKilledOtherPlayer);
        PlayerControllerInteractionManager.Instance.OnPlayerStartedObjective.AddListener(GameSettings.gameMode.OnPlayerStartedObjective);
        PlayerControllerInteractionManager.Instance.OnPlayerScoredObjective.AddListener(GameSettings.gameMode.OnPlayerScoredObjective);
    }

    private void OnDisable()
    {
        PlayerControllerInteractionManager.Instance.OnPlayerHitOtherPlayer.RemoveListener(OnPlayerHitOtherPlayer_Basic_GamemodeIndependent);
        PlayerControllerInteractionManager.Instance.OnPlayerKilledOtherPlayer.RemoveListener(OnPlayerKilledOtherPlayer_Basic_GamemodeIndependent);

        PlayerControllerInteractionManager.Instance.OnPlayerKilledOtherPlayer.RemoveListener(GameSettings.gameMode.OnPlayerKilledOtherPlayer);
        PlayerControllerInteractionManager.Instance.OnPlayerStartedObjective.RemoveListener(GameSettings.gameMode.OnPlayerStartedObjective);
        PlayerControllerInteractionManager.Instance.OnPlayerScoredObjective.RemoveListener(GameSettings.gameMode.OnPlayerScoredObjective);
    }

    private void OnPlayerKilledOtherPlayer_Basic_GamemodeIndependent(PlayerController attacker, PlayerController victim)
    {
        attacker.playerStats.kills++;
        victim.playerStats.deaths++;
        victim.OnPlayerDied.Invoke();
        StartCoroutine(DeactivateAndRespawnPlayerAfterShortTimeDelay(victim));

        var popUp = Instantiate(popUpPlusTextPreFab.gameObject, FindObjectOfType<Canvas>().transform)
            .GetComponent<PlusTextPopup>();

        popUp.SetValue("+1");
        popUp.transform.position = dynamicCamera.cam.WorldToScreenPoint(attacker.transform.position + popUp.offset);
    }

    private void OnPlayerHitOtherPlayer_Basic_GamemodeIndependent(PlayerController attacker, PlayerController victim)
    {
        attacker.playerStats.hitsGiven++;
        victim.playerStats.hitsTaken++;
        victim.OnPlayerWasShot.Invoke();
        victim.VisualizeTakingDamage();
    }

    void GoToJoinRoom()
    {
        SceneManager.LoadScene("JoinRoom");
    }

    IEnumerator DeactivateAndRespawnPlayerAfterShortTimeDelay(PlayerController player)
    {
        player.gameObject.SetActive(false);
        player.playerStats.ResetStatsOnPlayerDeath();
        yield return new WaitForSeconds(GameSettings.RespawnDelay);
        player.gameObject.SetActive(true);
        player.transform.position = map.GetGoodSpawnPoint(player);

    }
}
