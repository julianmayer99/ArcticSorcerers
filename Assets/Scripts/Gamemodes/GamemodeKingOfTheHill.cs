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
    public GameObject firstKingOfTheHillCollectable;

    public GamemodeBase.Gamemode ModeName { get; set; } = GamemodeBase.Gamemode.KingOfTheHill;
    public List<Team> TeamScores { get; set; }
    public int ScoreLimit { get; set; } = 76;
    public int RoundLimit { get; set; } = 1;
    public UnityEvent OnGameEnd { get; set; }
    public UnityEvent OnRoundEnd { get; set; }

    [HideInInspector] public PlayerController currentKingOfTheHill;

    private void Awake()
    {
        fixedUpdateSecondLimit = Mathf.CeilToInt(1 / Time.fixedDeltaTime);
    }

    public void InitializeInLevel()
    {
        GameModeUi = Instantiate(uiPreFab, FindObjectOfType<Canvas>().transform).GetComponent<IGameModeUi>();
        Instantiate(firstKingOfTheHillCollectable, GameManager.Instance.map.initial_KOTH_Spawn.position, Quaternion.identity);
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
        if (currentKingOfTheHill != null)
            currentKingOfTheHill.playerUI.IsKingOfTheHill = false;

        currentKingOfTheHill = player;
        player.playerUI.IsKingOfTheHill = true;
    }

    public void ResetForNextRound()
    {
        GamemodeBase.ResetForNextRound();
        currentKingOfTheHill = null;
    }

    public void OnModeSpawnedInJoinRoom()
    {
        GamemodeBase.AutoAssignTeams();
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

    private int fixedUpdateSecondLimit; // = Mathf.CeilToInt(1 / Time.fixedDeltaTime);
    private int fixedUppdateCounter = 0;
    private int secondsPassed = 0;

    private void FixedUpdate()
    {
        if (currentKingOfTheHill == null)
            return;

        if (!GameSettings.gameHasStarted)
            return;

        fixedUppdateCounter++;
        if (fixedUppdateCounter == fixedUpdateSecondLimit)
        {
            fixedUppdateCounter = 0;
            OnTimeSecondPassed();
            secondsPassed++;
            GameModeUi.UpdateTimeLeftTimer(GamemodeBase.GetTimeString(TimeLimitSeconds - secondsPassed));
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

    public int NumberOfTeams => PlayerConfigurationManager.Instance.Players.Count;

    public int RoundsLeftToPlay { get; set; }
    public int TimeLimitSeconds { get; set; } = 150;
}
