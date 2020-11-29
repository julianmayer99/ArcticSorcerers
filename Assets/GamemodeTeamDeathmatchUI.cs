using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamemodeTeamDeathmatchUI : MonoBehaviour, IGameModeUi
{
    public TeamNamePanel[] teamPanels;

    public List<Team> CorrespondingTeams { get; set; }

    public void InitializeUI(List<Team> teams)
    {
        CorrespondingTeams = teams;

        for (int i = 0; i < teamPanels.Length; i++)
        {
            teamPanels[i].gameObject.SetActive(CorrespondingTeams.Count < i);
            if (CorrespondingTeams.Count < i)
            {
                teamPanels[i].InitializeUI(CorrespondingTeams[i]);
            }
        }
    }

    public void UpdateUI()
    {
        foreach (var team in teamPanels)
        {
            team.UpdateUI();
        }
    }
}
