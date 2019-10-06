using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemy : Enemy
{
    public float igniteDistance;
    bool ignited = false;
    public Animator anim;
    public float delay;
    float waitTime;
    public float radius;
    public float force;

    public GameObject explosionEffect;


    void Update()
    {
        if (distance <= igniteDistance && !ignited)
        {
            anim.SetBool("isIgnited", true);
            ignited = true;
            waitTime = delay;
        }

        if (ignited && waitTime > 0f)
        {
            waitTime -= Time.fixedDeltaTime;
        }
        else if (ignited)
        {
            Explode();
        }
    }

    void Explode()
    {
        GameObject expl = Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D obj in colliders)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce((obj.transform.position - transform.position).normalized * force);
            }
        }

        if ((Player.transform.position - transform.position).magnitude < radius)
        {
            Player.GetComponent<PlayerController>().Damage(damage);
        }

        Destroy(expl, 3f);
        Destroy(gameObject);
    }

    override protected void Die()
    {
        Player.GetComponent<PlayerController>().Reward(XP);
        StageManager.GetComponent<StageManager>().OneDie();
        Explode();
    }
}
