using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualScoreBoardController : MonoBehaviour
{
    public ScoreBoardTeamEntry[] teamEntries;
    public TextMeshProUGUI txt_extraInfo0_header;
    public TextMeshProUGUI txt_extraInfo1_header;
    public TextMeshProUGUI txt_extraInfo2_header;

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

        txt_extraInfo0_header.text = GameSettings.gameMode.GameModeUi.GetScoreBoardColumnHeader(0);
        txt_extraInfo1_header.text = GameSettings.gameMode.GameModeUi.GetScoreBoardColumnHeader(1);
        txt_extraInfo2_header.text = GameSettings.gameMode.GameModeUi.GetScoreBoardColumnHeader(2);
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
