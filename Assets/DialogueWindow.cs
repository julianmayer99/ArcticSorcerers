using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI txt_headerTitle;

    public void SetTitle(string title)
    {
        txt_headerTitle.text = title;
    }
}
