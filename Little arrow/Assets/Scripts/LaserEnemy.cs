using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : Enemy
{
    public Transform firePoint;
    public Weapon pew;
    float shootingTimer = 0f;

    public LineRenderer lr;


    void Update()
    {
        if (distance < viewDistance && shootingTimer <= 0f)
        {
            ShootRaycast();
        }

        if (shootingTimer > 0f)
        {
            shootingTimer -= Time.fixedDeltaTime;
        }
    }

    void ShootRaycast()
    {
        shootingTimer = 1 / pew.fireRate;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, lr.startWidth, firePoint.up);
        foreach (RaycastHit2D hit in hits)
        {
            PlayerController player = hit.collider.GetComponent<PlayerController>();
            if (player != null && (hit.collider.transform.position - transform.position).magnitude < 20f)
            {
                player.Damage(pew.raycastDamage);
            }
        }

        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, firePoint.position + firePoint.up * 100);
        StartCoroutine(FlashLineRenderer());
    }

    IEnumerator FlashLineRenderer()
    {
        lr.enabled = true;

        yield return new WaitForSeconds(0.1f);

        lr.enabled = false;
    }
}
