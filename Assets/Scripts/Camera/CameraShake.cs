using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CameraSystem _cameraSystem;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _cameraSystem = GetComponent<CameraSystem>();
    }

    public void StartShake(float duration, float intensity)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, intensity));
    }

    private IEnumerator ShakeRoutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float strength = Mathf.Lerp(intensity, 0f, elapsed / duration);

            _cameraSystem.SetShakeOffset(new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0f
            ));

            elapsed += Time.deltaTime;
            yield return null;
        }

        _cameraSystem.SetShakeOffset(Vector3.zero);
    }
}
