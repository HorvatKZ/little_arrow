using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int HP = 3;
    public int damage = 1;
    public int XP = 1;
    public GameObject DeathEffect;
    public GameObject StageManager;

    public GameObject Player;

    public float viewDistance = 10f;
    public float stopDistance = 0f;
    public float retreatDistance = 0f;
    public Rigidbody2D rb;
    public float Speed = 5f;

    public float smoothness = 0.5f;
    Vector2 zero = Vector2.zero;
    protected Vector2 direction;
    protected float distance;
    float turnTimer;

    public void Damage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Die();
        }
    }

    protected void Start()
    {
        Player = GameObject.Find("Player");
        StageManager = GameObject.Find("StageManager");
    }

    private void FixedUpdate()
    {
        //Calculating direction
        if (Player != null)
        {
            distance = (transform.position - Player.transform.position).magnitude;
            if (distance < viewDistance)
            {
                direction = new Vector2(
                    Player.transform.position.x - transform.position.x,
                    Player.transform.position.y - transform.position.y
                    ).normalized;
            }
        }

        if (Player == null)
        {
            distance = 100f;
        }

        //Moving according to distance
        if (distance < viewDistance && distance > stopDistance)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, direction * Speed, ref zero, smoothness);
            transform.up = rb.velocity.normalized;
        }
        else if (distance < retreatDistance)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, -direction * Speed, ref zero, smoothness);
            transform.up = rb.velocity.normalized;
        }
        else if (distance >= viewDistance)
        {
            if (turnTimer > 0f)
            {
                turnTimer -= Time.fixedDeltaTime;
            }
            else
            {
                direction = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                    ).normalized;
                turnTimer = 1f;
            }

            rb.velocity = Vector2.SmoothDamp(rb.velocity, direction * Speed / 2, ref zero, smoothness);
            transform.up = rb.velocity.normalized;
        }
        else
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref zero, smoothness);
        }
    }

    virtual protected void Die()
    {
        GameObject death = Instantiate(DeathEffect, transform.position, transform.rotation);
        death.transform.localScale = transform.localScale;
        Player.GetComponent<PlayerController>().Reward(XP);
        StageManager.GetComponent<StageManager>().OneDie();
        Destroy(death, 2f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Player")
        {
            collision.collider.GetComponent<PlayerController>().Damage(damage);
        }   
    }
}
