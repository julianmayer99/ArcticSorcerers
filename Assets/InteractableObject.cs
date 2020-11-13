using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class InteractableObject : MonoBehaviour
{
    [System.Serializable]
    public class PlayerControllerEvent : UnityEvent<PlayerController> { };

    public PlayerControllerEvent OnPlayerEnters;
    public PlayerControllerEvent OnPlayerLeaves;
    [Space]
    public PlayerControllerEvent OnPlayerInteracted;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.transform.GetComponent<PlayerController>();

        if (player != null)
        {
            OnPlayerEnters.Invoke(player);
            player.selectedInteractable = this;
            Debug.Log("Player entered interactable Area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.transform.GetComponent<PlayerController>();

        if (player != null)
        {
            OnPlayerLeaves.Invoke(player);
            player.selectedInteractable = null;
            Debug.Log("Player left interactable Area");
        }
    }
}
