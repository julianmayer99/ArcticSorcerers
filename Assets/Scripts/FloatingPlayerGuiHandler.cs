using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlayerGuiHandler : MonoBehaviour
{
    [SerializeField] private SVGImage img_teamPanel;
    [SerializeField] private TextMeshProUGUI txt_playerName;
    [SerializeField] private Image img_nameTextBackground;
    [SerializeField] private Image[] ammunitionKnobs;
    [SerializeField] private Vector3 guiOffset;
    private Transform followObject;
    private PlayerController attatchedPlayer;
    private Camera sceneCamera;
    public Transform aimIndicator;

    public void SetUpFloatingGui(PlayerController player, Camera camera)
    {
        this.followObject = player.transform;
        this.sceneCamera = camera;

        attatchedPlayer = player;
        txt_playerName.text = player.config.info.name;
        if (player.config.Color != null)
        {
            UpdatePlayerColor();
        }

        this.transform.SetAsFirstSibling();

        UpdateTeamColor();
    }

    public void UpdateTeamColor()
    {
        if (GameSettings.gameMode.IsTeamBased)
        {
            img_teamPanel.gameObject.SetActive(true);
            img_teamPanel.color = ColorManager.Instance.teamColors[attatchedPlayer.config.Team.teamId].ui_color_normal;
            img_teamPanel.sprite = ColorManager.Instance.teamColors[attatchedPlayer.config.Team.teamId].teamIcon;
        }
        else
        {
            img_teamPanel.gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerProfileUi()
    {
        txt_playerName.text = attatchedPlayer.config.info.name;
    }

    public void UpdateAmmunitionReserveCount(int shotsLeft)
    {
        for (int i = 0; i < ammunitionKnobs.Length; i++)
        {
            ammunitionKnobs[i].gameObject.SetActive(i < shotsLeft);
        }
    }

    private void LateUpdate()
    {
        if (followObject != null)
        {
            transform.position = sceneCamera.WorldToScreenPoint(followObject.position + guiOffset);
        }
    }

    public void UpdatePlayerColor()
    {
        img_nameTextBackground.color = attatchedPlayer.config.Color.ui_color_dark;
        foreach (var item in ammunitionKnobs)
            item.color = attatchedPlayer.config.Color.ui_color_dark;
    }

    public bool IsAiming
    {
        get
        {
            return aimIndicator.gameObject.activeSelf;
        }
        set
        {
            aimIndicator.gameObject.SetActive(value);
        }
    }
}
