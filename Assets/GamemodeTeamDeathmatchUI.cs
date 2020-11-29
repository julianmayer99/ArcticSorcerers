using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamemodeTeamDeathmatchUI : MonoBehaviour, IGameModeUi
{
    public TeamNamePanel[] teamPanels;
    public GameObject roundEndPanel;
    public GameObject gameEndPanel;

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
    }

    public void ShowGameEndScreen()
    {
        roundEndPanel.SetActive(true);
        // TODO: Select() a button for controller navigation
    }

    public void ShowRoundEndScreen()
    {
        gameEndPanel.SetActive(true);
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
