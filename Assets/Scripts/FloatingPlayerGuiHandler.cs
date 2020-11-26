using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPlayerGuiHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_playerName;
    [SerializeField] private Image img_nameTextBackground;
    [SerializeField] private GameObject[] ammunitionKnobs;
    [SerializeField] private Vector3 guiOffset;
    private Transform followObject;
    private PlayerController attatchedPlayer;
    private Camera sceneCamera;

    public void SetUpFloatingGui(PlayerController player, Camera camera)
    {
        this.followObject = player.transform;
        this.sceneCamera = camera;

        attatchedPlayer = player;
        txt_playerName.text = player.config.playerName;
        if (player.config.Color != null)
            img_nameTextBackground.color = player.config.Color.ui_color_dark;
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

    public void OnPlayerColorChanged()
    {
        img_nameTextBackground.color = attatchedPlayer.config.Color.ui_color_dark;
    }
}
