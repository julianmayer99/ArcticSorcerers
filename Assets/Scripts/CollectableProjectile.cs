using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableProjectile : MonoBehaviour
{
    private bool hasBeenCollected = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (hasBeenCollected)
        {
            Destroy(gameObject);
            return;
        }

        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        player.ChangeAmmunnitionReserve(1);
        // This bool variable is nescessary to prevent
        // multi-collisions as Destroy takes some time
        hasBeenCollected = true;

        Destroy(gameObject);
    }
}
