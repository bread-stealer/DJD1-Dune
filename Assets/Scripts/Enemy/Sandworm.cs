using System.Collections;
using UnityEngine;

public class Sandworm : Enemy
{
    [Header("Attack")]
    [SerializeField] private float warningDuration = 2f;
    [SerializeField] private float burstDuration = 1f;
    [SerializeField] private float disappearDelay = 3f;
    [SerializeField] private float burstHeight = 3f;

    [Header("References")]
    [SerializeField] private SpriteRenderer wormSprite;
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private WormHead wormHead;

    [Header("Stats")]
    [SerializeField] private LayerMask playerLayer;

    private enum WormState { Idle, Warning, Emerging, Underground }
    private WormState _state = WormState.Idle;

    private Vector3 _groundPosition;
    private Vector3 _originalSpriteScale;

    protected override EnemyStats CreateStats()
    {
        return new EnemyStats(
            maxHealth: 1f,
            moveSpeed: 0f,
            damage: 0f,
            attackRange: 0f,
            detectionRange: 0f,
            attackCooldown: 0f,
            playerLayer: playerLayer
        );
    }

    protected override void Awake()
    {
        base.Awake();

        _originalSpriteScale = wormSprite.transform.localScale;

        if (wormSprite != null)
            wormSprite.enabled = false;

        if (wormHead != null)
            wormHead.SetActive(false);
    }

    private void Start()
    {
        _groundPosition = transform.position;
    }

    // Called by SandwormManager when the player has been on sand long enough
    public void Trigger(Vector3 playerPosition)
    {
        if (_state != WormState.Idle) return;
        StartCoroutine(EmergeSequence(playerPosition));
    }

    public bool IsIdle => _state == WormState.Idle;

    // Sandworm has no autonomous behaviour > manager drives it
    protected override void EvaluateState() { }

    private IEnumerator EmergeSequence(Vector3 playerPosition)
    {
        _state = WormState.Warning;

        // Snap to player's X along the ground
        transform.position = new Vector3(playerPosition.x, _groundPosition.y, _groundPosition.z);

        if (cameraShake != null)
            cameraShake.StartShake(warningDuration, 0.3f);

        if (sandParticles != null)
            sandParticles.Play();

        yield return new WaitForSeconds(warningDuration);

        // Emerge
        _state = WormState.Emerging;
        wormSprite.transform.localScale = _originalSpriteScale;
        wormSprite.enabled = true;

        if (wormHead != null)
            wormHead.SetActive(true);

        yield return StartCoroutine(BurstAnimation());
        yield return new WaitForSeconds(burstDuration);

        if (wormHead != null)
            wormHead.SetActive(false);

        // Burrow back
        _state = WormState.Underground;
        wormSprite.enabled = false;

        if (sandParticles != null)
            sandParticles.Stop();

        yield return new WaitForSeconds(disappearDelay);
        _state = WormState.Idle;
    }

    private IEnumerator BurstAnimation()
    {
        Vector3 groundPosition = transform.position;
        Vector3 peakPosition = groundPosition + Vector3.up * burstHeight;

        float elapsed = 0f;
        float halfDuration = burstDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            transform.position = Vector3.Lerp(groundPosition, peakPosition, easedT);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float easedT = Mathf.Pow(t, 2f);
            transform.position = Vector3.Lerp(peakPosition, groundPosition, easedT);
            yield return null;
        }

        wormSprite.transform.localScale = new Vector3(_originalSpriteScale.x, -_originalSpriteScale.y, _originalSpriteScale.z);
        transform.position = groundPosition;
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 8f);
    }
#endif
}