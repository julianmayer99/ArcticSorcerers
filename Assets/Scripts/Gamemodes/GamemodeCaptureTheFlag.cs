using UnityEngine;
using System.Collections;
using Assets.Scripts.Items;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

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
    private int roundsLeftToPlay;
    public UnityEvent OnGameEnd { get; set; }
    public UnityEvent OnRoundEnd { get; set; }

    public void InitializeInLevel()
    {
        GameModeUi = Instantiate(uiPreFab.GameObject, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();
        roundsLeftToPlay = RoundLimit;

        if (OnGameEnd == null)
            OnGameEnd = new UnityEvent();

        if (OnRoundEnd == null)
            OnRoundEnd = new UnityEvent();

        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            var team = TeamScores.SingleOrDefault(t => t.teamId == player.config.Team.teamId);
            if (team == null)
                TeamScores.Add(player.config.Team);
        }

        for (int i = 0; i < GameManager.Instance.map.ctfFlagSpawns.Length; i++)
        {
            ctfFlags.Add(Instantiate(ctfFlagPreFab.gameObject, GameManager.Instance.map.ctfFlagSpawns[i].position, Quaternion.identity)
                .GetComponent<CtfFlag>()
            );
        }
    }

    public void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim)
    {
    }

    public void OnPlayerScoredObjective(PlayerController player)
    {
        player.config.Team.score += pointsForScoringObjective;
        CheckIfATeamHasWon();
        GameModeUi.UpdateUI();
    }

    public void OnPlayerStartedObjective(PlayerController player)
    {
    }

    void CheckIfATeamHasWon()
    {
        if (ATeamHasPassedTheScoreLimit)
        {
            roundsLeftToPlay--;

            if (roundsLeftToPlay <= 0)
            {
                OnGameEnd.Invoke();
                GameModeUi.ShowGameEndScreen();
            }
            else
            {
                OnRoundEnd.Invoke();
                ResetForNextRound();
                GameModeUi.ShowRoundEndScreen();
            }
        }
    }

    public void ResetForNextRound()
    {
        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            player.transform.position = GameManager.Instance.map.GetGoodGameStartSpawnPoint(player);
            player.playerStats.ResetStatsOnPlayerDeath();
        }
    }

    public void OnModeSpawnedInJoinRoom()
    {
        throw new System.NotImplementedException();
    }

    bool ATeamHasPassedTheScoreLimit
    {
        get
        {
            foreach (var team in TeamScores)
            {
                if (team.score >= ScoreLimit)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public IGameModeUi GameModeUi { get; set; }
    public GameObject GameObject => gameObject;

    public bool IsTeamBased => true;

    public int NumberOfTeams => 2;

    public int RoundsLeftToPlay { get; set; }
}
