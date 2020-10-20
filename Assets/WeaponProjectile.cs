using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponProjectile : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody rb;
    public GameObject collectablePreFab;
    public GameObject particleBurstPreFab;
    [HideInInspector] public PlayerController shotFromPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.config.playerId == shotFromPlayer.config.playerId)
            {
                // Is players own bullet
                return;
            }

            player.OnPlayerHasBeenShot(shotFromPlayer);
        }

        Instantiate(collectablePreFab, transform.position, Quaternion.identity);
        Instantiate(particleBurstPreFab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
