using UnityEngine;
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

    public IGameModeUi uiPreFab;

    public CtfFlag ctfFlagPreFab;
    public List<CtfFlag> ctfFlags = new List<CtfFlag>();

    public PlayerConfigurationManager.Gamemode ModeName { get; set; } = PlayerConfigurationManager.Gamemode.CaptureTheFlag;
    public List<Team> TeamScores { get; set; } = new List<Team>();
    public int ScoreLimit { get; set; } = 3;
    public int RoundLimit { get; set; } = 2;
    public UnityEvent OnGameEnd { get; set; }
    public UnityEvent OnRoundEnd { get; set; }

    public void InitializeInLevel()
    {
        GameModeUi = Instantiate(uiPreFab.GameObject, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();

        GamemodeBase.InitializeInLevel();

        for (int i = 0; i < GameSettings.gameMode.NumberOfTeams; i++)
        {
            var flag = Instantiate(ctfFlagPreFab.gameObject, GameManager.Instance.map.ctfFlagSpawns[i].position, Quaternion.identity)
                .GetComponent<CtfFlag>();
            flag.Initialize(GameSettings.gameMode.TeamScores[i], GameManager.Instance.map.ctfFlagSpawns[i].position);
            ctfFlags.Add(flag);
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
        GamemodeBase.OnPlayerScoredObjective(player, pointsForScoringObjective);
        player.gamemodeExtraInfo.CarryingFlag = null;
    }

    // Pick Up The Flag
    public void OnPlayerStartedObjective(PlayerController player)
    {
        GameModeUi.UpdateUI();
    }

    public void ResetForNextRound()
    {
        GamemodeBase.ResetForNextRound();
        // TODO: Reset Flag Positions
    }

    public void OnModeSpawnedInJoinRoom()
    {
        GamemodeBase.AutoAssignTeams();
    }

    public IGameModeUi GameModeUi { get; set; }
    public GameObject GameObject => gameObject;

    public bool IsTeamBased => true;

    public int NumberOfTeams => 2;

    public int RoundsLeftToPlay { get; set; }
}
