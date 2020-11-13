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
    [SerializeField] private GameObject readyPanel;

    private float ignoreInputTime = 1.5f;
    private bool inputEnabled;
    private bool isReady = false;

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

        if (isReady)
        {
            PlayerConfigurationManager.Instance.AttemptGameStart(playerIndex);
            return;
        }

        isReady = true;

        readyPanel.SetActive(true);
        PlayerConfigurationManager.Instance.ReadyPlayer(playerIndex);
    }

    private void OnActionLeftPerformed(InputAction.CallbackContext context)
    {
        bool buttonDown = context.ReadValue<float>() > 0;
        if (buttonDown)
            PickPreviousCharaacter();
    }
    private void OnActionRightPerformed(InputAction.CallbackContext context)
    {
        bool buttonDown = context.ReadValue<float>() > 0;
        if (buttonDown)
            PickNextCharaacter();
    }

    private void OnPerformActionPerformed(InputAction.CallbackContext context)
    {
        bool buttonDown = context.ReadValue<float>() > 0;
        if (buttonDown)
        {
            ReadyPlayer();
            Debug.Log("Show previous");
        }
    }

    public void SetListeners()
    {
        actionLeft.performed += OnActionLeftPerformed;
        actionRight.performed += OnActionRightPerformed;
        actionConfirm.performed += OnPerformActionPerformed;

        actionLeft.Enable();
        actionRight.Enable();
        actionConfirm.Enable();
    }

    private void OnDisable()
    {
        actionLeft.performed -= OnActionLeftPerformed;
        actionRight.performed -= OnActionRightPerformed;
        actionConfirm.performed -= OnPerformActionPerformed;

        actionLeft.Disable();
        actionRight.Disable();
        actionConfirm.Disable();
    }

    public void PickNextCharaacter() => swipeMenu.ShowNext();
    public void PickPreviousCharaacter() => swipeMenu.ShowPrevious();
}
