using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareEnemy : Enemy
{
    public Transform firePoint;
    public Weapon pew;
    float shootingTimer = 0f;

    void Update()
    {
        if (distance < viewDistance && shootingTimer <= 0f)
        {
            Shoot(direction);
        }

        if (shootingTimer > 0f)
        {
            shootingTimer -= Time.fixedDeltaTime;
        }
    }

    void Shoot(Vector2 direction)
    {
        pew.Shoot(firePoint, direction);
        shootingTimer = 1 / pew.fireRate;
    }
}
