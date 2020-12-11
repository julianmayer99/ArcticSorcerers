using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardTeamEntry : MonoBehaviour
{
    public ScoreBoardPlayerEntry[] playerEntries;

    public void Initialize(int teamId)
    {
        int playerCount = GameSettings.gameMode.TeamScores[teamId].players.Count;

        for (int i = 0; i < playerEntries.Length; i++)
        {
            if (playerCount > i)
            {
                playerEntries[i].gameObject.SetActive(true);
                playerEntries[i].Initialize(GameSettings.gameMode.TeamScores[teamId].players[i]);
            } else
                playerEntries[i].gameObject.SetActive(false);

        }
    }

    public void UpdateUI()
    {
        foreach (var entry in playerEntries)
        {
            if (entry.gameObject.activeSelf)
                entry.UpdateUI();
        }
    }
}
