using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardTeamEntry : MonoBehaviour
{
    public bool displayEndGameStats = false;
    [Space]
    public GameObject wonRoundsDescriptionText;
    public TextMeshProUGUI txt_teamScore;
    public TextMeshProUGUI txt_gamemodeScoreLimit;
    public SVGImage teamIcon;
    public ScoreBoardPlayerEntry[] playerEntries;
    private int teamId;

    public void Initialize(int teamId)
    {
        this.teamId = teamId;
        if (GameSettings.gameMode.IsTeamBased)
            wonRoundsDescriptionText.SetActive(displayEndGameStats);

        if (GameSettings.gameMode.IsTeamBased)
            txt_gamemodeScoreLimit.text = displayEndGameStats
                ? GameSettings.gameMode.TeamScores[teamId].totalScore + " Punkte insgesamt"
                : GameSettings.gameMode.ScoreLimit.ToString();

        if (GameSettings.gameMode.IsTeamBased)
            teamIcon.sprite = ColorManager.Instance.teamColors[teamId].teamIcon;

        if (!GameSettings.gameMode.IsTeamBased)
        {
            var img = GetComponent<Image>();
            img.color = GameSettings.gameMode.TeamScores[teamId].Players.First().config.Color.ui_color_dark;
            img.CrossFadeAlpha(.8f, 0f, true);
        }

        if (GameSettings.gameMode.IsTeamBased)
        {
            int playerCount = GameSettings.gameMode.TeamScores[teamId].Players.Count;
            for (int i = 0; i < playerEntries.Length; i++)
            {
                if (playerCount > i)
                {
                    playerEntries[i].gameObject.SetActive(true);
                    playerEntries[i].Initialize(GameSettings.gameMode.TeamScores[teamId].Players[i]);
                }
                else
                    playerEntries[i].gameObject.SetActive(false);
            }
        }
        else
        {
            playerEntries[0].Initialize(GameSettings.gameMode.TeamScores[teamId].Players.First());
        }


        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (var entry in playerEntries)
        {
            if (entry.gameObject.activeSelf)
                entry.UpdateUI();
        }
        if (GameSettings.gameMode.IsTeamBased)
            txt_teamScore.text = displayEndGameStats
                ? GameSettings.gameMode.TeamScores[teamId].wonRounds.ToString()
                : GameSettings.gameMode.TeamScores[teamId].score.ToString();
    }
}
