using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChangeProfileController : MonoBehaviour
{
    public TMP_InputField inp_newPlayerName;
    public DialogueWindow attatchedDialogueWindow;
    public SavedPlayerItem[] savedPlayerPanels;

    private void OnEnable()
    {
        UpdateListSorting();
    }

    public void UpdateListSorting()
    {
        var matches = (from player in PlayerLevelingManager.Players
                      where player.Name.ToLower().Contains(inp_newPlayerName.text.ToLower())
                      select player).ToArray();

        for (int i = 0; i < savedPlayerPanels.Length; i++)
        {
            savedPlayerPanels[i].gameObject.SetActive(i < matches.Length);
            if (i < matches.Length)
            {
                savedPlayerPanels[i].Initialize(matches[i]);
            }
        }
    }

    public void SavePlayerNameButtonClicked()
    {
        var playerInfo = PlayerLevelingManager.Instance.GetOrCreatePlayer(inp_newPlayerName.text);
        var player = attatchedDialogueWindow.invokedPlayers.First();
        player.config.info = playerInfo;
        player.playerUI.UpdatePlayerProfileUi();

        attatchedDialogueWindow.CloseWindow();
        inp_newPlayerName.text = string.Empty;
    }
}
