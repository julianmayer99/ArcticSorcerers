﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Items;

public class ScoreBoardPlayerEntry : MonoBehaviour
{
    public TextMeshProUGUI txt_playerName;
    public TextMeshProUGUI txt_playerLevel;
    public TextMeshProUGUI txt_extraInfo0;
    public TextMeshProUGUI txt_extraInfo1;
    public TextMeshProUGUI txt_extraInfo2;

    private PlayerController player;

    public void Initialize(PlayerController player)
    {
        this.player = player;
        txt_playerName.text = player.config.info.name;
        txt_playerLevel.text = player.config.info.Level.ToString();

    }

    public void UpdateUI()
    {
        if (player == null)
            return;

        txt_extraInfo0.text = GameSettings.gameMode.GameModeUi.GetScoreBoardRowValueForPlayer(player, 0);
        txt_extraInfo1.text = GameSettings.gameMode.GameModeUi.GetScoreBoardRowValueForPlayer(player, 1);
        txt_extraInfo2.text = GameSettings.gameMode.GameModeUi.GetScoreBoardRowValueForPlayer(player, 2);
    }
}
