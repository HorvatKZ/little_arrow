using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public StageManager stg;
    public GameObject deathEffect;

    public float speed;
    public Rigidbody2D rb;

    public Transform firePoint;

    Vector2 move;
    public float smoothness;

    public Weapon pew;
    float shootingTimer;
    Vector2 zero = Vector2.zero;

    int maxHP;
    int HP;
    int XP = 0;
    int requiredXP;

    public Animator anim;
    bool noDamage;
    public float noDamageTime = 2f;
    float noDamageTimer;

    public Text HpText;
    public Transform HpAnchor;
    public Text XpText;
    public Transform XpAnchor;

    public Sprite[] levelSprite;
    public int[] levelHP;
    public int[] levelXP;
    public float[] levelMultiplier;
    int level = 0;

    public GameObject LevelUpEffect;
    public Animator lvlupAnim;
    float lvlupAnimTimer = 0f;
    public SpriteRenderer spr;

    public LineRenderer lr;

    public void Damage(int amount)
    {
        if (!noDamage)
        {
            HP -= amount;
            anim.SetBool("isDamage", true);
            noDamageTimer = noDamageTime;
            noDamage = true;
            HpText.text = HP.ToString();
            HpAnchor.localScale = new Vector3((float)HP/maxHP, 1f, 1f);
            if (HP <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        GameObject death = Instantiate(deathEffect, transform.position, transform.rotation);
        death.transform.localScale = transform.localScale;
        Destroy(death, 2f);
        Destroy(gameObject);
        stg.GameOver(XP);
    }

    public void Reward(int value)
    {
        XP += value;
        XpAnchor.localScale = new Vector3((float)(XP - levelXP[level]) / levelXP[level + 1], 1f, 1f);
        XpText.text = XP.ToString();
    }

    private void Start()
    {
        maxHP = levelHP[0];
        requiredXP = levelXP[1];
        HP = maxHP;
        HpText.text = HP.ToString();
        XpText.text = "0";
    }

    void FixedUpdate()
    {
        //Rotating
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0f, 0f, 10f));
        move = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
            );
        transform.up = move;

        //Moving
        Vector2 movement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            ).normalized;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, movement * speed, ref zero, smoothness);

        //Shooting
        if (shootingTimer > 0)
        {
            shootingTimer -= Time.fixedDeltaTime;
        }
        else
        {
            shootingTimer = 0f;
        }

        if (pew.shootsRaycasts && Input.GetMouseButtonDown(0))
        {
            ShootRaycast();
        }
        else if (shootingTimer == 0f && ((Input.GetMouseButtonDown(0) && !pew.isAuto) || (Input.GetMouseButton(0) && pew.isAuto)))
        {
            Shoot();
        }

        //No Damage
        if (noDamageTimer > 0)
        {
            noDamageTimer -= Time.fixedDeltaTime;
        }
        else
        {
            noDamage = false;
            anim.SetBool("isDamage", false);
        }

        //Level up
        if (XP >= requiredXP)
        {
            LevelUp();
        }

        //Level Up anim timer
        if (lvlupAnimTimer > 0f)
        {
            lvlupAnimTimer -= Time.fixedDeltaTime;
        }
        else
        {
            lvlupAnimTimer = 0f;
            lvlupAnim.SetBool("levelUp", false);
        }
    }

    void Shoot()
    {
        pew.Shoot(firePoint, move.normalized);
        shootingTimer = 1 / (pew.fireRate * levelMultiplier[level]);
    }

    void ShootRaycast()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, lr.startWidth, firePoint.up);
        foreach (RaycastHit2D hit in hits)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null && (hit.collider.transform.position - transform.position).magnitude < 20f)
            {
                enemy.Damage(pew.raycastDamage);
            }
        }

        lr.SetPosition(0, firePoint.position);
        lr.SetPosition(1, firePoint.position + firePoint.up * 100);

        StartCoroutine(FlashLineRenderer());
    }

    IEnumerator FlashLineRenderer()
    {
        lr.enabled = true;

        yield return new WaitForSeconds(0.02f);

        lr.enabled = false;
    }

    void LevelUp()
    {
        ++level;
        spr.sprite = levelSprite[level];
        maxHP = levelHP[level];
        HP = maxHP;
        requiredXP += levelXP[level + 1];
        Reward(0);
        HpText.text = HP.ToString();
        HpAnchor.localScale = new Vector3((float)HP / maxHP, 1, 1);

        lvlupAnim.SetBool("levelUp", true);
        lvlupAnimTimer = 2f;
        GameObject lvlup = Instantiate(LevelUpEffect, transform.position, transform.rotation);
        Destroy(lvlup, 2f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20f);
        foreach (Collider2D obj in colliders)
        {
            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(100);
            }
        }
    }
}
