using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingPlayerGuiHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt_playerName;
    [SerializeField] private GameObject[] ammunitionKnobs;
    [SerializeField] private Vector3 guiOffset;
    private Transform followObject;
    private PlayerController attatchedPlayer;
    private Camera sceneCamera;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetUpFloatingGui(PlayerController player, Camera camera)
    {
        this.followObject = player.transform;
        this.sceneCamera = camera;

        txt_playerName.text = player.config.playerName;
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
}
