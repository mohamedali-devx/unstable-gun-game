using UnityEngine;

public class FlameThrower : Weapon
{
    [Header("Flamethrower Settings")]
    public ParticleSystem flameParticles; // اربطها من الـ Inspector على السلاح نفسه
    public GameObject flameHitBox; // object collider يحرق العدو

    void Start()
    {
        if (flameHitBox != null)
            flameHitBox.SetActive(false); // نخليه مغلق بالبداية
    }

    public override void Shoot()
    {
        if (flameParticles != null && !flameParticles.isPlaying)
            flameParticles.Play();

        if (flameHitBox != null)
            flameHitBox.SetActive(true); // تفعيل الـ object لحرق العدو
    }

    public void StopFlame()
    {
        if (flameParticles != null && flameParticles.isPlaying)
            flameParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (flameHitBox != null)
            flameHitBox.SetActive(false); // إيقاف تفعيل object عند رفع اليد
    }
}
