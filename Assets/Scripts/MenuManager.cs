using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject uiContainerMenu;
    public GameObject uiContainerJoinRoom;

    private void Start()
    {
        DynamicMultiTargetCamera.instance.enabled = false;

        SetMenuUiActive(PlayerConfigurationManager.Instance.Players.Count <= 0);
        if (PlayerConfigurationManager.Instance.Players.Count <= 0)
            FindObjectOfType<MainMenuDollyMovement>().SetToStart();
        else
            FindObjectOfType<MainMenuDollyMovement>().SetToEnd();
    }

    public void SkipMainMenuScreen()
    {
        if (PlayerConfigurationManager.Instance.Players.Count <= 0)
            FindObjectOfType<MainMenuDollyMovement>().StartMoveFromStartToEnd(() =>
            {
                DynamicMultiTargetCamera.instance.enabled = true;
            });
        else
        {
            FindObjectOfType<MainMenuDollyMovement>().SetToEnd();
            DynamicMultiTargetCamera.instance.enabled = false;
        }

        SetMenuUiActive(false);
    }

    void SetMenuUiActive(bool enableMenuUi)
    {
        uiContainerJoinRoom.SetActive(!enableMenuUi);
        uiContainerMenu.SetActive(enableMenuUi);
    }
}
