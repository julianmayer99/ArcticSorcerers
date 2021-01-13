using Assets.Scripts.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialCollectableKingOfTheHill : MonoBehaviour
{
    private bool hasBeenCollectedByPlayer = false;
    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenCollectedByPlayer)
            return;

        var player = other.GetComponent<PlayerController>();

        if (player == null)
            return;

        hasBeenCollectedByPlayer = true;
        FindObjectOfType<GamemodeKingOfTheHill>().currentKingOfTheHill = player;
    }
}
