using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private Coroutine _stopCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Stop(float duration)
    {
        if (_stopCoroutine != null)
            StopCoroutine(_stopCoroutine);
        _stopCoroutine = StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f;
        // WaitForSecondsRealtime ignores timeScale, so the freeze ends
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}
