using System.Collections;
using UnityEngine;

public class Sandworm : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private float triggerRadius = 8f;
    [SerializeField] private float emergeDelay = 5f;
    [SerializeField] private float escapeRadius = 12f;

    [Header("Attack")]
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float burstDuration = 1f;
    [SerializeField] private float disappearDelay = 3f;
    // How high the worm rises above the ground
    [SerializeField] private float burstHeight = 3f;

    [Header("References")]
    [SerializeField] private SpriteRenderer wormSprite;
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private CameraShake cameraShake;

    [Header("Layer")]
    [SerializeField] private LayerMask playerLayer;

    private enum WormState { Waiting, Countdown, Warning, Emerging, Underground }
    private WormState _state = WormState.Waiting;

    private Transform _player;
    private float _countdownTimer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
        }
        else
        {
            Debug.LogError("[Sandworm] No Player found in scene.");
        }

        // Worm starts hidden underground
        if (wormSprite != null)
            wormSprite.enabled = false;
    }

    private void Update()
    {
        if (_player == null) return;

        switch (_state)
        {
            case WormState.Waiting:
                CheckPlayerInRange();
                break;

            case WormState.Countdown:
                TickCountdown();
                break;

            // Warning, Emerging, Underground handled by coroutines
        }
    }

    private void CheckPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance <= triggerRadius)
        {
            _countdownTimer = emergeDelay;
            _state = WormState.Countdown;
            Debug.Log("[Sandworm] Player entered area > countdown started.");
        }
    }

    private void TickCountdown()
    {
        float distance = Vector2.Distance(transform.position, _player.position);

        // If player escapes the trigger radius, reset
        if (distance > escapeRadius)
        {
            _state = WormState.Waiting;
            Debug.Log("[Sandworm] Player escaped, resetting.");
            return;
        }

        _countdownTimer -= Time.deltaTime;
        if (_countdownTimer <= 0f)
            StartCoroutine(EmergeSequence());
    }

    private IEnumerator EmergeSequence()
    {
        _state = WormState.Warning;

        // Snap worm to player's current X position along the ground
        Vector3 emergePos = new Vector3(_player.position.x, transform.position.y, transform.position.z);
        transform.position = emergePos;

        // Warning phase > camera shake before burst
        if (cameraShake != null)
            cameraShake.StartShake(warningDuration, 0.3f);

        if (sandParticles != null)
            sandParticles.Play();

        yield return new WaitForSeconds(warningDuration);

        // Emerge phase > worm bursts up from ground
        _state = WormState.Emerging;
        if (wormSprite != null)
            wormSprite.enabled = true;

        // WormHead collider handles the kill > no distance check needed here
        yield return StartCoroutine(BurstAnimation());

        yield return new WaitForSeconds(burstDuration);

        // Burrow back underground
        _state = WormState.Underground;
        if (wormSprite != null)
            wormSprite.enabled = false;

        if (sandParticles != null)
            sandParticles.Stop();

        // Wait then reset so worm can trigger again
        yield return new WaitForSeconds(disappearDelay);
        _state = WormState.Waiting;
        Debug.Log("[Sandworm] Worm reset waiting.");
    }

    private IEnumerator BurstAnimation()
    {
        if (wormSprite == null) yield break;

        Vector3 groundPosition = transform.position;
        Vector3 peakPosition = groundPosition + Vector3.up * burstHeight;

        float elapsed = 0f;
        float halfDuration = burstDuration * 0.5f;

        // Rise up > fast at start, slows at peak
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;

            // Ease out
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            wormSprite.transform.position = Vector3.Lerp(groundPosition, peakPosition, easedT);
            yield return null;
        }

        elapsed = 0f;

        // Fall back down > slow at start, fast at bottom
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;

            // Ease in
            float easedT = Mathf.Pow(t, 2f);
            wormSprite.transform.position = Vector3.Lerp(peakPosition, groundPosition, easedT);
            yield return null;
        }

        wormSprite.transform.position = groundPosition;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Yellow = trigger radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        // Red = escape radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, escapeRadius);
    }
#endif
}