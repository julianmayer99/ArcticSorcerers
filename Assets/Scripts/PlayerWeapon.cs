using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public PlayerController attatchedPlayer;
    public WeaponProjectile projectilePreFab;
    public Transform shootPoint;

    public void Shoot()
    {
        var projectile = Instantiate(projectilePreFab, shootPoint.position, shootPoint.rotation).GetComponent<WeaponProjectile>();
        projectile.shotFromPlayer = attatchedPlayer;
    }
}
