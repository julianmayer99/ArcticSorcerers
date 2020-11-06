using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int playerIndex;

    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField] private SwipeMenu swipeMenu;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;

    public InputAction actionLeft;
    public InputAction actionRight;
    public InputAction actionConfirm;

    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
        titleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    public void SetCharacter(int id)
    {
        if (!inputEnabled) return;

        PlayerConfigurationManager.Instance.SetPlayerCharacter(playerIndex, id);
    }

    void Update()
    {
        if (!inputEnabled && Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SelectColor(PlayerConfiguration.PlayerColor color)
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.SetPlayerColor(playerIndex, color);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
    }

    public void SetListeners()
    {
        actionLeft.performed += (context) =>
        {
            bool buttonDown = context.ReadValue<float>() > 0;
            if (buttonDown)
                PickPreviousCharaacter();
        };

        actionRight.performed += (context) =>
        {
            bool buttonDown = context.ReadValue<float>() > 0;
            if (buttonDown)
            {
                PickNextCharaacter();
                Debug.Log("Show next");
            }
        };

        actionConfirm.performed += (context) =>
        {
            bool buttonDown = context.ReadValue<float>() > 0;
            if (buttonDown)
            {
                ReadyPlayer();
                Debug.Log("Show previous");
            }
        };

        actionLeft.Enable();
        actionRight.Enable();
        actionConfirm.Enable();
    }

    public void PickNextCharaacter() => swipeMenu.ShowNext();
    public void PickPreviousCharaacter() => swipeMenu.ShowPrevious();
}
