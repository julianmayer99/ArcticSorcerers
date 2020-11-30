﻿using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamNamePanel : MonoBehaviour
{
    public TextMeshProUGUI txt_TeamName;
    public TextMeshProUGUI txt_Score;
    private Team correspondingTeam;

    public void InitializeUI(Team team)
    {
        correspondingTeam = team;
        txt_TeamName.text = "Team " + team.teamId; // TODO:
    }

    public void UpdateUI()
    {
        txt_Score.text = correspondingTeam.score + "/" + GameSettings.gameMode.ScoreLimit;
    }
}