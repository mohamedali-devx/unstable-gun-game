using UnityEngine;
using System.Collections;

public class EnemyVerticalJumper : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public float moveSpeed = 2f; // إذا عايز يتحرك يمين ويسار على المنصة
    public float detectionRange = 5f;
    private bool canJump = true;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Effects")]
    public ParticleSystem deathParticle;

    private Rigidbody2D rb;
    private bool isFlashing = false;
    private Color originalColor;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange && canJump)
        {
            JumpTowardsPlayer();
        }
    }

    void JumpTowardsPlayer()
    {
        canJump = false;

        // اتجاه اللاعب بالنسبة للعدو
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, jumpForce);

        // إرجاع القدرة على القفز بعد نصف ثانية
        StartCoroutine(ResetJump(0.5f));
    }

    IEnumerator ResetJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        canJump = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (!isFlashing)
            StartCoroutine(FlashRed());

        // knockback خفيف عند الإصابة
        rb.linearVelocity = new Vector2(-rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);

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
            case "Player":
                // لو عايز تقتل اللاعب مباشرة
                collision.gameObject.SendMessage("Die");
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
            case "Player":
                collider.gameObject.SendMessage("Die");
                break;
        }
    }
}
