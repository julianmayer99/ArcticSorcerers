using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Gamemodes;

public class GamemodeTeamDeathmatchUI : MonoBehaviour, IGameModeUi
{
    public TextMeshProUGUI txt_timer;
    public TeamNamePanel[] teamPanels;
    public DialogueWindow roundEndPanelPreFab;
    public DialogueWindow gameEndPanelPreFab;
    
    private DialogueWindow roundEndPanel;
    private DialogueWindow gameEndPanel;

    public List<Team> CorrespondingTeams { get; set; }
    public GameObject GameObject => gameObject;

    public string GamemodeName { get; set; } = "Team Deathmatch";

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

        GamemodeUiBase.InitializeTeamBasedUI(teamPanels);

        gameEndPanel = Instantiate(gameEndPanelPreFab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<DialogueWindow>();
        gameEndPanel.gameObject.SetActive(false);
        // TODO: spawn round end screen

        Debug.Log("<color=green>Gamemode Ui Initialized: Team Deathmatch</color>");
    }

    public void ShowGameEndScreen()
    {
        gameEndPanel.gameObject.SetActive(true);
        var teamWithMorePoints = GameSettings.gameMode.TeamScores.OrderByDescending(t => t.score).First();
        teamWithMorePoints.wonRounds++;
        gameEndPanel.SetTitle(teamWithMorePoints.Name + " hat das Spiel gewonnen.");
    }

    public void ShowRoundEndScreen()
    {
        roundEndPanelPreFab.gameObject.SetActive(true);
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
