using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine _shakeCoroutine;

    public void StartShake(float duration, float intensity)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, intensity));
    }

    private IEnumerator ShakeRoutine(float duration, float intensity)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Intensity fades out toward the end
            float strength = Mathf.Lerp(intensity, 0f, elapsed / duration);

            transform.localPosition = originalPosition + new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0f
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
