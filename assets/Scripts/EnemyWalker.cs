using UnityEngine;
using System.Collections;

public class EnemyWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red; // اللون عند الإصابة
    public float flashDuration = 0.1f;    // مدة الوميض

    [Header("Particles")]
    public ParticleSystem deathParticle;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, rb.linearVelocity.y);

        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider == null)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (!isFlashing)
            StartCoroutine(FlashRed());

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
            spriteRenderer.color = originalColor;
        }
        isFlashing = false;
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


    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}
