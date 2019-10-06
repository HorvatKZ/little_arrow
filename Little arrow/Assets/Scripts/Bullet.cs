using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;

    public bool isEnemy;

    void Start()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && !isEnemy)
        {
            collision.GetComponent<Enemy>().Damage(1);
            Destroy(gameObject);
        }
        if (collision.tag == "Player" && isEnemy)
        {
            collision.GetComponent<PlayerController>().Damage(1);
            Destroy(gameObject);
        }
        if (collision.tag == "Obstacle")
        {
            Destroy(gameObject);
        }

    }
}
