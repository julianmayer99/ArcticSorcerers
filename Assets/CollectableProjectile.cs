using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null)
            return;

        // TODO: Add mana to player

        Destroy(gameObject);
    }
}
