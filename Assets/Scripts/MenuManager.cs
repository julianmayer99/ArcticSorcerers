using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject uiContainerMenu;
    public GameObject uiContainerJoinRoom;

    private void Start()
    {
        Camera.main.GetComponent<DynamicMultiTargetCamera>().enabled = false;

        SetMenuUiActive(PlayerConfigurationManager.Instance.Players.Count <= 0);
        if (PlayerConfigurationManager.Instance.Players.Count <= 0)
            FindObjectOfType<MainMenuDollyMovement>().SetToStart();
        else
        {
            FindObjectOfType<MainMenuDollyMovement>().SetToEnd();
            Camera.main.GetComponent<DynamicMultiTargetCamera>().enabled = true;
        }
    }

    public void SkipMainMenuScreen()
    {
        if (PlayerConfigurationManager.Instance.Players.Count <= 0)
            FindObjectOfType<MainMenuDollyMovement>().StartMoveFromStartToEnd(() =>
            {
                Camera.main.GetComponent<DynamicMultiTargetCamera>().enabled = true;
            });
        else
        {
            FindObjectOfType<MainMenuDollyMovement>().SetToEnd();
            Camera.main.GetComponent<DynamicMultiTargetCamera>().enabled = true;
        }

        SetMenuUiActive(false);
    }

    void SetMenuUiActive(bool enableMenuUi)
    {
        uiContainerJoinRoom.SetActive(!enableMenuUi);
        uiContainerMenu.SetActive(enableMenuUi);
    }
}
