using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

public class TeamNamePanel : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float backgroundPanelAlphaValue = .8f;
    public SVGImage teamIcon;
    public TextMeshProUGUI txt_TeamName;
    public TextMeshProUGUI txt_Score;
    public TextMeshProUGUI txt_ScoreLimit;
    private Team correspondingTeam;

    public void InitializeUI(Team team)
    {
        correspondingTeam = team;
        teamIcon.sprite = ColorManager.Instance.teamColors[team.teamId].teamIcon;
        txt_ScoreLimit.text = "/ " + GameSettings.gameMode.ScoreLimit.ToString();
        txt_TeamName.text = "Team " + team.teamId; // TODO:
        GetComponent<UnityEngine.UI.Image>().color = ColorManager.Instance.teamColors[team.teamId].ui_color_dark;
    }

    public void UpdateUI()
    {
        txt_Score.text = correspondingTeam.score.ToString();
    }
}
