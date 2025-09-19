using UnityEngine;
using System.Collections;

public class EnemyFlyerLook : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public LayerMask obstacleLayer;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool canMove = true;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Particles")]
    public ParticleSystem deathParticle;

    [Header("Knockback Settings")]
    public float knockbackForce = 2f;
    public float knockbackDuration = 0.2f;

    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance <= detectionRange)
                {
                    // اتجاه العدو ناحية اللاعب
                    Vector2 dir = (player.position - transform.position).normalized;
                    moveDirection = dir;

                    // Rotate sprite to face player
                    if (dir.x > 0)
                        spriteRenderer.flipX = false; // مواجهة لليمين
                    else
                        spriteRenderer.flipX = true;  // مواجهة لليسار

                    // Raycast لتجنب الحواجز
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, obstacleLayer);
                    if (hit.collider != null)
                        moveDirection = Vector2.zero; // توقف مؤقت عند وجود حاجز
                }
                else
                {
                    moveDirection = Vector2.zero;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void FixedUpdate()
    {
        if (canMove)
            rb.linearVelocity = moveDirection * moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (!isFlashing)
            StartCoroutine(FlashRed());

        StartCoroutine(Knockback());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashRed()
    {
        isFlashing = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
        }
        isFlashing = false;
    }

    IEnumerator Knockback()
    {
        canMove = false;
        Vector2 knockDir = -(player.position - transform.position).normalized;
        rb.linearVelocity = knockDir * knockbackForce;

        yield return new WaitForSeconds(knockbackDuration);
        canMove = true;
    }

    void Die()
    {
        if (deathParticle != null)
            Instantiate(deathParticle, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "bullet":
                TakeDamage(1);
                Destroy(collision.gameObject);
                break;
            case "bullet2":
                TakeDamage(2);
                Destroy(collision.gameObject);
                break;
            case "bullet3":
                TakeDamage(5);
                Destroy(collision.gameObject);
                break;
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "bullet":
                TakeDamage(1);
                Destroy(collider.gameObject);
                break;
            case "bullet2":
                TakeDamage(2);
                Destroy(collider.gameObject);
                break;
            case "bullet3":
                TakeDamage(5);
                Destroy(collider.gameObject);
                break;
            case "enemy":
                Die(); // إذا العدو لمس اللاعب
                break;
        }
    }

}
