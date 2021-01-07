using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndScreen : MonoBehaviour
{
    public void QuitGameButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }

}
