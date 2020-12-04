﻿using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;

public class GamemodeTeamDeathmatchUI : MonoBehaviour, IGameModeUi
{
    public TeamNamePanel[] teamPanels;
    public DialogueWindow roundEndPanelPreFab;
    public DialogueWindow gameEndPanelPreFab;
    
    private DialogueWindow roundEndPanel;
    private DialogueWindow gameEndPanel;

    public List<Team> CorrespondingTeams { get; set; }
    public GameObject GameObject => gameObject;

    public void InitializeUI(List<Team> teams)
    {
        CorrespondingTeams = teams;

        for (int i = 0; i < teamPanels.Length; i++)
        {
            teamPanels[i].gameObject.SetActive(i < CorrespondingTeams.Count);
            if (i < CorrespondingTeams.Count)
            {
                teamPanels[i].InitializeUI(CorrespondingTeams[i]);
            }
        }

        gameEndPanel = Instantiate(gameEndPanelPreFab.gameObject, FindObjectOfType<Canvas>().transform).GetComponent<DialogueWindow>();
        gameEndPanel.gameObject.SetActive(false);
        // TODO: spawn round end screen
    }

    public void ShowGameEndScreen()
    {
        // TODO: Select() a button for controller navigation
        gameEndPanel.gameObject.SetActive(true);
        var teamWithMorePoints = GameSettings.gameMode.TeamScores.OrderByDescending(t => t.score).First();
        gameEndPanel.SetTitle(teamWithMorePoints.Name + " hat das Spiel gewonnen.");
    }

    public void ShowRoundEndScreen()
    {

        // roundEndPanelPreFab.gameObject.SetActive(true);
        // TODO: Select() a button for controller navigation
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
