using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CloseableDialogueWindow : MonoBehaviour
{
    public InputAction BackAction;
    public UnityEvent OnBackButtonClickedWhenActive;
    [SerializeField] private bool injectBackActionListeners = true;
    [SerializeField] private bool injectForAllPlayer = false;

    private List<PlayerController> invokedPlayers = new List<PlayerController>();

    private void Awake()
    {
        if (OnBackButtonClickedWhenActive == null)
            OnBackButtonClickedWhenActive = new UnityEvent();

        if (injectForAllPlayer)
        {
            invokedPlayers.Clear();
            foreach (var item in PlayerConfigurationManager.Instance.Players)
            {
                invokedPlayers.Add(item);
            }
        }
    }

    public void ShowDialogueWindow(PlayerController player)
    {
        invokedPlayers.Clear();
        invokedPlayers.Add(player);
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (injectBackActionListeners)
            foreach (var player in invokedPlayers)
                player.OnBackActionTriggered.AddListener(CloseWindow);
    }

    private void OnDisable()
    {
        if (injectBackActionListeners)
            foreach (var player in invokedPlayers)
                player.OnBackActionTriggered.RemoveListener(CloseWindow);
    }

    public void CloseWindow()
    {
        OnBackButtonClickedWhenActive.Invoke();
    }

}
