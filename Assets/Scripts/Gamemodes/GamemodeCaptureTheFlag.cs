﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Items;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Assets.Scripts.Gamemodes;

/// <summary>
/// Two teams play against each other. The objective is, to capture the enemies flag and return it to the own base.
/// </summary>
public class GamemodeCaptureTheFlag : MonoBehaviour, IGameMode
{
    public int pointsForScoringObjective = 1;
    public int pointsForLoosingObjective = 0;

    public GameObject uiPreFab;

    public CtfFlag ctfFlagPreFab;
    public List<CtfFlag> ctfFlags = new List<CtfFlag>();

    public GamemodeBase.Gamemode ModeName { get; set; } = GamemodeBase.Gamemode.CaptureTheFlag;
    public List<Team> TeamScores { get; set; } = new List<Team>();
    public int ScoreLimit { get; set; } = 1;
    public int RoundLimit { get; set; } = 2;
    public UnityEvent OnGameEnd { get; set; }
    public UnityEvent OnRoundEnd { get; set; }

    public void InitializeInLevel()
    {
        GameModeUi = Instantiate(uiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();

        GamemodeBase.InitializeInLevel();

        for (int i = 0; i < GameSettings.gameMode.NumberOfTeams; i++)
        {
            var flag = Instantiate(ctfFlagPreFab.gameObject, GameManager.Instance.activeMap.ctfFlagSpawns[i].position, Quaternion.identity)
                .GetComponent<CtfFlag>();
            var team = GameSettings.gameMode.TeamScores.SingleOrDefault(t => t.teamId == i);
            if (team != null)
            {
                flag.Initialize(team, GameManager.Instance.activeMap.ctfFlagSpawns[team.teamId].position);
                ctfFlags.Add(flag);
            }
        }
    }

    public void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim)
    {
        if (victim.gamemodeExtraInfo.CarryingFlag != null)
        {
            victim.gamemodeExtraInfo.CarryingFlag.carryingPlayer = null;
            victim.gamemodeExtraInfo.CarryingFlag = null;

            attacker.gamemodeExtraInfo.killedAFlagCarrier++;
        }

        if (attacker.gamemodeExtraInfo.CarryingFlag != null)
        {
            attacker.gamemodeExtraInfo.killedAsFlagCarrier++;
        }
    }

    public void OnPlayerScoredObjective(PlayerController player)
    {
        var flag = player.gamemodeExtraInfo.CarryingFlag;
        GamemodeBase.OnPlayerScoredObjective(player, pointsForScoringObjective);

        flag.carryingPlayer = null;
        flag.ReturnOwnFlagToHomeBase();

        player.gamemodeExtraInfo.CarryingFlag = null;
        player.gamemodeExtraInfo.flagsCaptured++;
    }

    // Pick Up The Flag
    public void OnPlayerStartedObjective(PlayerController player)
    {
        player.gamemodeExtraInfo.flagsTaken++;
        GameModeUi.UpdateUI();
    }

    public void ResetForNextRound()
    {
        GamemodeBase.ResetForNextRound();
        foreach (var flag in ctfFlags)
        {
            flag.ResetFlagOnRoundEnd();
        }
    }

    public void OnModeSpawnedInJoinRoom()
    {
        GamemodeBase.AutoAssignTeams();
    }

    public void SaveStatsOnGameEnd()
    {
        
    }

    public IGameModeUi GameModeUi { get; set; }
    public GameObject GameObject => gameObject;

    public bool IsTeamBased => true;

    public int NumberOfTeams => 2;

    public int RoundsLeftToPlay { get; set; }
    public int TimeLimitSeconds { get; set; } = 300;
}
