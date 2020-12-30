using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponseVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject darkTintPanel;
    public static ResponseVisualizer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void TintScreenBlackOnLongWaitTime()
    {
        darkTintPanel.SetActive(true);
    }
}

