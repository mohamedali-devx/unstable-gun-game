using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Weapon[] weapons;
    private int currentWeaponIndex = 0;
    public float weaponSwitchInterval = 5f;
    private float nextSwitchTime;

    [Header("Weapon Pivot")]
    public Transform weaponPivot;

    [Header("Player Sprites")]
    public GameObject normalSprite;
    public GameObject shootingSprite;
    public float shootSpriteTime = 0.5f;

    [Header("Camera Shake Settings")]
    public CameraFollow cameraFollow;
    public float shakeAmount = 0.1f;
    public float shakeDuration = 0.1f;

    private Rigidbody2D rb;
    private float nextFireTime = 0f;
    private Weapon currentWeapon;

    [Header("Player Face Settings")]
    public GameObject playerFace1;
    public GameObject playerFace2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (normalSprite != null) normalSprite.SetActive(true);
        if (shootingSprite != null) shootingSprite.SetActive(false);

        if (weapons != null && weapons.Length > 0)
        {
            currentWeaponIndex = Random.Range(0, weapons.Length);
            currentWeapon = weapons[currentWeaponIndex];
            ActivateCurrentWeapon();
        }

        nextSwitchTime = Time.time + weaponSwitchInterval;

        if (cameraFollow != null && cameraFollow.target == null)
            cameraFollow.target = this.transform;
    }

    void Update()
    {
        AimWeapon();

        if (currentWeapon != null)
        {
            float currentFireRate = currentWeapon.fireRate;
            float currentRecoil = currentWeapon.recoilForce;

            if (currentWeapon is FlameThrower flameWeapon)
            {
                if (Input.GetMouseButton(0))
                {
                    flameWeapon.Shoot();
                    ApplyRecoil(currentRecoil);
                    HandleShootSprite();
                    HandleCameraShake();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    flameWeapon.StopFlame();
                }
            }
            else
            {
                if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
                {
                    currentWeapon.Shoot();
                    ApplyRecoil(currentRecoil);
                    nextFireTime = Time.time + currentFireRate;
                    HandleShootSprite();
                    HandleCameraShake();
                }
            }
        }

        HandleWeaponSwitching();
    }

    void ApplyRecoil(float force)
    {
        Transform mp = (currentWeapon != null && currentWeapon.muzzlePoint != null) ? currentWeapon.muzzlePoint : weaponPivot;
        if (mp == null) return;

        Vector3 mouseScreen = Input.mousePosition;
        float camZ = Mathf.Abs(Camera.main.transform.position.z - mp.position.z);
        mouseScreen.z = camZ;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 shootDir = (mouseWorld - mp.position).normalized;
        rb.linearVelocity = -shootDir * force;
    }

    void AimWeapon()
    {
        if (weaponPivot == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - weaponPivot.position);
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        // ✅ Flip السلاح والوجه حسب اتجاه الماوس
        if (direction.x < 0)
        {
            weaponPivot.localScale = new Vector3(1, -1, 1);
            if (playerFace1 != null)
                playerFace1.transform.localScale = new Vector3(-1, 1, 1);
            if (playerFace2 != null)
                playerFace2.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            weaponPivot.localScale = new Vector3(1, 1, 1);
            if (playerFace1 != null)
                playerFace1.transform.localScale = new Vector3(1, 1, 1);
            if (playerFace2 != null)
                playerFace2.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void HandleShootSprite()
    {
        if (normalSprite != null) normalSprite.SetActive(false);
        if (shootingSprite != null) shootingSprite.SetActive(true);
        CancelInvoke(nameof(ResetSprite));
        Invoke(nameof(ResetSprite), shootSpriteTime);
    }

    void HandleCameraShake()
    {
        if (cameraFollow != null)
            cameraFollow.StartShake(shakeDuration, shakeAmount);
    }

    void HandleWeaponSwitching()
    {
        if (weapons == null || weapons.Length <= 1) return;
        if (Time.time >= nextSwitchTime)
        {
            if (currentWeapon is FlameThrower pf) pf.StopFlame();

            // ✅ اختيار سلاح عشوائي مختلف عن الحالي
            int newIndex;
            do
            {
                newIndex = Random.Range(0, weapons.Length);
            } while (newIndex == currentWeaponIndex);

            currentWeaponIndex = newIndex;
            currentWeapon = weapons[newIndex];
            ActivateCurrentWeapon();

            nextSwitchTime = Time.time + weaponSwitchInterval;
            nextFireTime = Time.time + 0.15f; // delay صغير بعد التبديل
        }
    }

    void ActivateCurrentWeapon()
    {
        if (weapons == null) return;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] is FlameThrower flame) flame.StopFlame();
            if (weapons[i] != null) weapons[i].gameObject.SetActive(i == currentWeaponIndex);
        }
    }

    void ResetSprite()
    {
        if (shootingSprite != null) shootingSprite.SetActive(false);
        if (normalSprite != null) normalSprite.SetActive(true);
    }

    // ✅ إضافة الموت عند لمس أي عدو
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("enemy"))
        {
            Die();
        }
    }

    [Header("Death Settings")]
    public ParticleSystem deathParticle; // اضغط واسحب particle من ال Inspector

    void Die()
    {
        // 1️⃣ اخفاء اللاعب فورًا
        gameObject.SetActive(false);

        // 2️⃣ اظهار particle عند موقع اللاعب
        if (deathParticle != null)
            Instantiate(deathParticle, transform.position, Quaternion.identity);

        // 3️⃣ إعادة تحميل المشهد بعد 1 ثانية (اختياري)
        Invoke(nameof(RestartScene), 1f);
    }

    void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

}
