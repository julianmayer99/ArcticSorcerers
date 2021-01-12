using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI txt_headerTitle;

    [HideInInspector] public InputAction BackAction;
    public UnityEvent OnBackButtonClickedWhenActive;
    [SerializeField] private bool injectBackActionListeners = true;
    [SerializeField] private bool injectForAllPlayer = false;
    public Button autoSelectWhenOpened;

    public List<PlayerController> invokedPlayers = new List<PlayerController>();

    private void Awake()
    {
        if (OnBackButtonClickedWhenActive == null)
            OnBackButtonClickedWhenActive = new UnityEvent();

        if (injectForAllPlayer)
        {
            invokedPlayers.Clear();
            foreach (var player in PlayerConfigurationManager.Instance.Players)
            {
                invokedPlayers.Add(player);
                player.playerControlsEnabled = false;
            }
        }
    }

    public void SetTitle(string title)
    {
        txt_headerTitle.text = title;
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
        StartCoroutine(ActivateButtonOnEndOfFrame());
    }

    private void OnDisable()
    {
        if (injectBackActionListeners)
            foreach (var player in invokedPlayers)
            {
                player.OnBackActionTriggered.RemoveListener(CloseWindow);
                player.playerControlsEnabled = true;
            }
    }

    IEnumerator ActivateButtonOnEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        if (autoSelectWhenOpened != null)
            autoSelectWhenOpened.Select();
    }

    public void CloseWindow()
    {
        OnBackButtonClickedWhenActive.Invoke();
    }

}
