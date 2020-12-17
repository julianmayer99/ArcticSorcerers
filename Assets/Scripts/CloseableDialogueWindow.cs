using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CloseableDialogueWindow : MonoBehaviour
{
    public InputAction BackAction;
    public UnityEvent OnBackButtonClickedWhenActive;
    [SerializeField] private bool injectBackActionListeners = true;

    private PlayerController invokedPlayer;

    private void Awake()
    {
        if (OnBackButtonClickedWhenActive == null)
            OnBackButtonClickedWhenActive = new UnityEvent();
    }

    public void ShowDialogueWindow(PlayerController player)
    {
        invokedPlayer = player;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (injectBackActionListeners)
            invokedPlayer.OnBackActionTriggered.AddListener(CloseWindow);
    }

    private void OnDisable()
    {
        if (injectBackActionListeners)
            invokedPlayer.OnBackActionTriggered.RemoveListener(CloseWindow);
    }

    public void CloseWindow()
    {
        OnBackButtonClickedWhenActive.Invoke();
    }
}
