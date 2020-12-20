using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlayerGuiHandler : MonoBehaviour
{
    [SerializeField] private Image img_teamPanel;
    [SerializeField] private TextMeshProUGUI txt_playerName;
    [SerializeField] private Image img_nameTextBackground;
    [SerializeField] private GameObject[] ammunitionKnobs;
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
        txt_playerName.text = player.config.playerName;
        if (player.config.Color != null)
            img_nameTextBackground.color = player.config.Color.ui_color_dark;

        UpdateTeamColor();
    }

    public void UpdateTeamColor()
    {
        if (GameSettings.gameMode.IsTeamBased)
        {
            img_teamPanel.gameObject.SetActive(true);
            img_teamPanel.color = ColorManager.Instance.teamColors[attatchedPlayer.config.Team.teamId].ui_color_normal;
        }
        else
        {
            img_teamPanel.gameObject.SetActive(false);
        }
    }

    public void UpdateAmmunitionReserveCount(int shotsLeft)
    {
        for (int i = 0; i < ammunitionKnobs.Length; i++)
        {
            ammunitionKnobs[i].SetActive(i < shotsLeft);
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
    }

    public void StartAiming()
    {
        aimIndicator.gameObject.SetActive(true);
    }

    public void StopAiming()
    {
        aimIndicator.gameObject.SetActive(false);
    }
}
