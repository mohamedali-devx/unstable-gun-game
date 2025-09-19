using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;       // اسحب اللاعب هنا أو يَعَيَّن تلقائياً من Movement
    public float smoothSpeed = 5f; // سرعة التتبع
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    // داخلي
    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeCoroutine;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset + shakeOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    // استدعي ديه من Movement: StartShake(duration, magnitude)
    public void StartShake(float duration, float magnitude)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;           // 0..1
            float damper = 1f - t;                 // يتناقص مع الوقت (يمكن تغييره لمنحنى مختلف)
            float x = (Random.value * 2f - 1f) * magnitude * damper;
            float y = (Random.value * 2f - 1f) * magnitude * damper;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset
        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}
