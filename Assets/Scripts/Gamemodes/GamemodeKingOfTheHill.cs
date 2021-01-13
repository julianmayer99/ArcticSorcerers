using Assets.Scripts.Gamemodes;
using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamemodeKingOfTheHill : MonoBehaviour, IGameMode
{

    public int pointsForKOTHEachSecond = 1;

    public GameObject uiPreFab;

    public PlayerConfigurationManager.Gamemode ModeName { get; set; } = PlayerConfigurationManager.Gamemode.KingOfTheHill;
    public List<Team> TeamScores { get; set; }
    public int ScoreLimit { get; set; } = 76;
    public int RoundLimit { get; set; } = 1;
    public UnityEvent OnGameEnd { get; set; }
    public UnityEvent OnRoundEnd { get; set; }

    private PlayerController currentKingOfTheHill;

    public void InitializeInLevel()
    {
        GameModeUi = Instantiate(uiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();

        GamemodeBase.InitializeInLevel();
    }

    public void OnPlayerKilledOtherPlayer(PlayerController attacker, PlayerController victim)
    {
        OnPlayerStartedObjective(attacker);
    }

    public void OnPlayerScoredObjective(PlayerController player)
    {
        GamemodeBase.OnPlayerScoredObjective(player, pointsForKOTHEachSecond);
    }

    public void OnPlayerStartedObjective(PlayerController player)
    {
        currentKingOfTheHill = player;
    }

    public void ResetForNextRound()
    {
        GamemodeBase.ResetForNextRound();
        currentKingOfTheHill = null;
    }

    public void OnModeSpawnedInJoinRoom()
    {
    }

    public void SaveStatsOnGameEnd()
    {
        GamemodeBase.CopyGenericPlayerStatsOnGameEnd();

        // TODO: klappt das mit den Teams?
        foreach (var team in GameSettings.gameMode.TeamScores)
        {
            foreach (var player in team.Players)
            {
                if (team.HasTheMostWonRounds)
                    player.config.info.xp += 25;

                player.config.info.xp += player.playerStats.kills * 5;
            }
        }
    }

    private int fixedUpdateSecondLimit = Mathf.CeilToInt(1 / Time.fixedDeltaTime);
    private int fixedUppdateCounter = 0;
    private void FixedUpdate()
    {
        if (currentKingOfTheHill == null)
            return;

        fixedUppdateCounter++;
        if (fixedUppdateCounter == fixedUpdateSecondLimit)
        {
            OnTimeSecondPassed();
        }
    }

    public void OnTimeSecondPassed()
    {
        if (currentKingOfTheHill != null)
            OnPlayerScoredObjective(currentKingOfTheHill);
    }

    public IGameModeUi GameModeUi { get; set; }
    public GameObject GameObject => gameObject;

    public bool IsTeamBased => false;

    public int NumberOfTeams => 2;

    public int RoundsLeftToPlay { get; set; }
    public int TimeLimitSeconds { get; set; } = 150;
}
