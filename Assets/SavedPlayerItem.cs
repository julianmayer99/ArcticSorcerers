using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SavedPlayerItem : MonoBehaviour
{
    public TextMeshProUGUI txt_playerName;
    public TextMeshProUGUI txt_level;
    private PlayerLevelingManager.PlayerInfo info;

    public void Initialize(PlayerLevelingManager.PlayerInfo info)
    {
        this.info = info;
        txt_playerName.text = info.name;
        txt_level.text = info.Level.ToString();
    }

    public void ButtonClicked()
    {
        FindObjectOfType<ChangeProfileController>().inp_newPlayerName.text = info.name;
    }
}
