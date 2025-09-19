using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform muzzlePoint;
    public GameObject muzzleFlashPrefab;

    [Header("Weapon Stats")]
    public float fireRate = 0.2f;    // معدل إطلاق النار
    public float recoilForce = 10f;  // قوة الارتداد

    [Header("Shotgun Settings")]
    public int pelletCount = 6;
    public float spreadAngle = 15f;

    public virtual void Shoot()
    {
        if (bulletPrefab == null || muzzlePoint == null) return;

        for (int i = 0; i < pelletCount; i++)
        {
            float randomAngle = Random.Range(-spreadAngle / 2, spreadAngle / 2);
            Quaternion pelletRotation = muzzlePoint.rotation * Quaternion.Euler(0, 0, randomAngle);

            GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, pelletRotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // ✅ خذ الاتجاه مباشرة من pelletRotation وليس من muzzlePoint.right
                Vector2 shootDirection = pelletRotation * Vector2.right;
                bulletScript.Launch(shootDirection);
            }
        }

        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            ParticleSystem ps = flash.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(flash, ps.main.duration + 0.1f);
            else
                Destroy(flash, 0.5f);
        }
    }
}
