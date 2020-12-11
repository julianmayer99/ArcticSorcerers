using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualScoreBoardController : MonoBehaviour
{
    public ScoreBoardTeamEntry[] teamEntries;

    private void Awake()
    {
        if (GameSettings.gameMode.IsTeamBased)
        {
            for (int i = 0; i < teamEntries.Length; i++)
            {
                teamEntries[i].Initialize(i);
            }
        }
        else
        {
            // TODO: UI für nicht teambasierte Spielmodi
        }
    }

    private void OnEnable()
    {
        if (GameSettings.gameMode.IsTeamBased)
        {
            for (int i = 0; i < teamEntries.Length; i++)
            {
                teamEntries[i].UpdateUI();
            }
        }
    }
}
