using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public float lifeTime = 2f;

    [Header("Effects")]
    public GameObject hitEffectPrefab; // 🎯 اسحب هنا الـ Particle من الـ Inspector

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // ✅ نحرك الرصاصة بالـ velocity
        rb.linearVelocity = direction.normalized * speed;

        // تمسح بعد فترة لو لمستش حاجة
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ✅ نعمل Particle عند مكان الاصطدام
        if (hitEffectPrefab != null)
        {
            ContactPoint2D contact = collision.GetContact(0);
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, contact.normal);
            GameObject effect = Instantiate(hitEffectPrefab, contact.point, rotation);

            // نمسح الـ Particle بعد ما يخلص
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(effect, 1f); // fallback لو مش ParticleSystem
            }
        }

        // ✅ تمسح الرصاصة لما تلمس الأرض أو العدو
        if (collision.gameObject.CompareTag("ground") || collision.gameObject.CompareTag("enemy"))
        {
            Destroy(gameObject);
        }
    }
}
