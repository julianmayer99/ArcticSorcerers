using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

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
        wonRoundsDescriptionText.SetActive(displayEndGameStats);

        txt_gamemodeScoreLimit.text = displayEndGameStats
            ? GameSettings.gameMode.TeamScores[teamId].totalScore + " Punkte insgesamt"
            : GameSettings.gameMode.ScoreLimit.ToString();
        teamIcon.sprite = ColorManager.Instance.teamColors[teamId].teamIcon;
        int playerCount = GameSettings.gameMode.TeamScores[teamId].Players.Count;

        for (int i = 0; i < playerEntries.Length; i++)
        {
            if (playerCount > i)
            {
                playerEntries[i].gameObject.SetActive(true);
                playerEntries[i].Initialize(GameSettings.gameMode.TeamScores[teamId].Players[i]);
            } else
                playerEntries[i].gameObject.SetActive(false);

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

        txt_teamScore.text = displayEndGameStats
            ? GameSettings.gameMode.TeamScores[teamId].wonRounds.ToString()
            : GameSettings.gameMode.TeamScores[teamId].score.ToString();
    }
}
