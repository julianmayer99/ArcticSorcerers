using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponProjectile : MonoBehaviour
{
    public float speed = 1f;
    public float smoothTime = .3f;
    public bool isSeeking = true;
    private Rigidbody rb;
    public GameObject collectablePreFab;
    public GameObject particleBurstPreFab;
    public PlayerController shotFromPlayer;

    private Vector3 m_Velocity = Vector3.zero;

    private ProjectileTarget lockedTarget;
    private bool hasLockedTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.velocity = transform.forward * speed * -1;
    }

    private void Update()
    {
        if (hasLockedTarget)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, (lockedTarget.transform.position - transform.position).normalized * speed, ref m_Velocity, smoothTime);
        }
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

            player.OnPlayerHasBeenShot(shotFromPlayer, collision.contacts[0].point);
        }

        Instantiate(collectablePreFab, transform.position, Quaternion.identity);
        Instantiate(particleBurstPreFab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponent<ProjectileTarget>();
        if (target == null)
            return;

        if (lockedTarget == null)
        {
            lockedTarget = target;
            hasLockedTarget = true;
        }
        else
        {
            lockedTarget = Vector3.Distance(transform.position, lockedTarget.transform.position) < Vector3.Distance(transform.position, target.transform.position)
                ? lockedTarget
                : target;
            hasLockedTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var target = other.gameObject.GetComponent<ProjectileTarget>();
        if (target == null)
            return;

        if (lockedTarget.gameObject.GetInstanceID() == target.gameObject.GetInstanceID())
        {
            hasLockedTarget = false;
        }
    }
}
