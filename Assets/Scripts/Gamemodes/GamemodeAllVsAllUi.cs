using UnityEngine;
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

    public string GamemodeName { get; set; } = "King Of The Hill";
    List<Team> IGameModeUi.CorrespondingTeams { get; set; }

    public string GetScoreBoardColumnHeader(int columnIndex)
    {
        switch (columnIndex)
        {
            // TODO: I18n
            case 0: return "Kills";
            case 1: return "Schüsse";
            case 2: return "Tode";
            default: return "-";
        }
    }

    public string GetScoreBoardRowValueForPlayer(PlayerController forPlayer, int columnIndex)
    {
        switch (columnIndex)
        {
            case 0: return forPlayer.playerStats.kills.ToString();
            case 1: return forPlayer.playerStats.shotsFired.ToString();
            case 2: return forPlayer.playerStats.deaths.ToString();
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

        Debug.Log("<color=green>Gamemode Ui Initialized: All vs. All</color>");
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
        gameEndPanel.SetTitle(teamWithMorePoints.Players.First().name + " hat das Spiel gewonnen.");
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
