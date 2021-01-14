using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualScoreBoardController : MonoBehaviour
{
    public GameObject teamScoreBoardBody;
    public GameObject allvAllScoreBoardBody;
    [Space]
    public ScoreBoardTeamEntry[] teamEntries;
    public ScoreBoardTeamEntry[] allvAllEntries;
    public TextMeshProUGUI txt_gamemodeName;
    public TextMeshProUGUI txt_extraInfo0_header;
    public TextMeshProUGUI txt_extraInfo1_header;
    public TextMeshProUGUI txt_extraInfo2_header;

    private void Awake()
    {
        txt_gamemodeName.text = GameSettings.gameMode.GameModeUi.GamemodeName;
        teamScoreBoardBody.SetActive(GameSettings.gameMode.IsTeamBased);
        allvAllScoreBoardBody.SetActive(!GameSettings.gameMode.IsTeamBased);

        if (GameSettings.gameMode.IsTeamBased)
        {
            for (int i = 0; i < teamEntries.Length; i++)
            {
                teamEntries[i].Initialize(i);
            }
        }
        else
        {
            for (int i = 0; i < allvAllEntries.Length; i++)
            {
                allvAllEntries[i].gameObject.SetActive(i < PlayerConfigurationManager.Instance.Players.Count);
                if (i < PlayerConfigurationManager.Instance.Players.Count)
                    allvAllEntries[i].Initialize(i);
            }
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
        else
        {
            for (int i = 0; i < allvAllEntries.Length; i++)
            {
                allvAllEntries[i].UpdateUI();
            }
        }
    }
}
