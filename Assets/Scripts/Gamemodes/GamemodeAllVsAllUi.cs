﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Items;
using System.Collections.Generic;
using Assets.Scripts.Gamemodes;
using System.Linq;
using TMPro;

public class GamemodeAllVsAllUi : MonoBehaviour, IGameModeUi
{
    public TextMeshProUGUI txt_timer;

    public TeamNamePanel[] teamPanels;
    public DialogueWindow roundEndPanelPreFab;
    public DialogueWindow gameEndPanelPreFab;

    private DialogueWindow roundEndPanel;
    private DialogueWindow gameEndPanel;

    public List<Team> CorrespondingTeams { get; set; }
    public GameObject GameObject => gameObject;

    List<Team> IGameModeUi.CorrespondingTeams { get; set; }

    public string GetScoreBoardColumnHeader(int columnIndex)
    {
        switch (columnIndex)
        {
            // TODO: I18n
            case 0: return "Erobert";
            case 1: return "Zur. gebracht";
            case 2: return "Kills";
            default: return "-";
        }
    }

    public string GetScoreBoardRowValueForPlayer(PlayerController forPlayer, int columnIndex)
    {
        switch (columnIndex)
        {
            case 0: return forPlayer.gamemodeExtraInfo.flagsCaptured.ToString();
            case 1: return forPlayer.gamemodeExtraInfo.flagsReturned.ToString();
            case 2: return forPlayer.playerStats.kills.ToString();
            default: return string.Empty;
        }
    }

    public void InitializeUI(List<Team> teams)
    {
        CorrespondingTeams = teams;

        GamemodeUiBase.InitializeNonTeamBasedUI(teamPanels);

        gameEndPanel = Instantiate(gameEndPanelPreFab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<DialogueWindow>();
        gameEndPanel.gameObject.SetActive(false);

        roundEndPanel = Instantiate(roundEndPanelPreFab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<DialogueWindow>();
        roundEndPanel.gameObject.SetActive(false);

        Debug.Log("<color=green>Gamemode Ui Initialized: Capture The Flag</color>");
    }

    public void ShowGameEndScreen()
    {
        gameEndPanel.gameObject.SetActive(true);
        GameSettings.gameHasStarted = false;
        foreach (var player in PlayerConfigurationManager.Instance.Players)
        {
            player.gameObject.SetActive(false);
        }
        var teamWithMorePoints = GameSettings.gameMode.TeamScores.OrderByDescending(t => t.score).First();
        teamWithMorePoints.wonRounds++;
        gameEndPanel.SetTitle(teamWithMorePoints.Name + " hat das Spiel gewonnen.");
    }

    public void ShowRoundEndScreen()
    {
        roundEndPanel.gameObject.SetActive(true);
    }

    public void UpdateTimeLeftTimer(string timeLeftText)
    {
        txt_timer.text = timeLeftText;
    }

    public void UpdateUI()
    {
        foreach (var team in teamPanels)
        {
            if (team.gameObject.activeSelf)
                team.UpdateUI();
        }
    }
}