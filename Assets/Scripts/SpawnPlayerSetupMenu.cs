using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject playerSetupMenuPrefab;

    private GameObject rootMenu;
    public PlayerInput input;

    private void Awake()
    {
        rootMenu = FindObjectOfType<JoinScreenController>().playerSelectContainer;

        if (rootMenu != null)
        {
            var menu = Instantiate(playerSetupMenuPrefab, rootMenu.transform).GetComponent<PlayerSetupMenuController>();
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.SetPlayerIndex(input.playerIndex);
            input.SwitchCurrentActionMap("SelectPlayer");
            menu.actionLeft = input.currentActionMap.FindAction("Back");
            menu.actionRight = input.currentActionMap.FindAction("Forward");
            menu.actionConfirm = input.currentActionMap.FindAction("Confirm");
            menu.SetListeners();
        }

        FindObjectOfType<PlayerConfigurationManager>().HandlePlayerJoin(input);

        input.onActionTriggered += LogInputAction;
        Debug.Log("Action listener injected.");
    }

    void LogInputAction(InputAction.CallbackContext context)
    {
        Debug.Log("Action triggered: " + context.action.name + " on device: " + context.control.device.name);
    }
}
