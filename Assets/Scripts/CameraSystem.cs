using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Trap Box Size")]
    [SerializeField] private float trapWidth = 4f;

    [Header("Vertical Trap (Asymmetric)")]
    [SerializeField] private float trapUp = 1.5f;
    [SerializeField] private float trapDown = 1.5f;

    [Header("Vertical Lookahead")]
    [SerializeField] private float lookaheadAmount = 1.5f;
    [SerializeField] private float lookaheadDelay = 0.8f;
    [SerializeField] private float lookaheadSpeed = 3f;

    [Header("Camera Bounds")]
    [SerializeField] private float fixedZ = -10f;
    [SerializeField] private bool useMinY = true;
    [SerializeField] private float minY = 0f;

    [Header("Follow Speed")]
    [SerializeField, Min(0f)] private float followSpeed = 8f;

    private Vector3 _targetPosition;
    private float _lookaheadOffset;
    private float _timeGrounded;
    private float _lastPlayerY;
    private bool _isGrounded;

    private float HalfWidth => trapWidth * 0.5f;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("[CameraSystem] Player Transform is not assigned.", this);
            enabled = false;
            return;
        }

        // Initialise camera position to player on start
        _targetPosition = new Vector3(player.position.x, player.position.y, fixedZ);
        transform.position = _targetPosition;
        _lastPlayerY = player.position.y;
    }

    private void LateUpdate()
    {
        UpdateGroundedState();
        UpdateLookahead();
        UpdateTrap();
        ApplyPosition();
    }

    private void UpdateGroundedState()
    {
        float playerY = player.position.y;
        float verticalSpeed = (playerY - _lastPlayerY) / Time.deltaTime;
        _lastPlayerY = playerY;

        // Consider grounded when vertical speed is near zero (standing on a floor)
        _isGrounded = Mathf.Abs(verticalSpeed) < 0.5f;

        if (_isGrounded)
            _timeGrounded += Time.deltaTime;
        else
            _timeGrounded = 0f;
    }

    private void UpdateLookahead()
    {
        // Only apply lookahead after player has been stable on a floor for lookaheadDelay seconds
        if (_isGrounded && _timeGrounded >= lookaheadDelay)
        {
            float targetOffset = player.position.y - _targetPosition.y;
            targetOffset = Mathf.Clamp(targetOffset, -lookaheadAmount, lookaheadAmount);
            _lookaheadOffset = Mathf.Lerp(_lookaheadOffset, targetOffset, lookaheadSpeed * Time.deltaTime);
        }
        else if (!_isGrounded)
        {
            // Reset lookahead while in the air
            _lookaheadOffset = Mathf.Lerp(_lookaheadOffset, 0f, lookaheadSpeed * Time.deltaTime);
        }
    }

    private void UpdateTrap()
    {
        Vector3 camPos = _targetPosition;
        Vector3 playerPos = player.position;

        // Horizontal trap — symmetric
        float deltaX = playerPos.x - camPos.x;
        if (deltaX > HalfWidth)
            camPos.x = playerPos.x - HalfWidth;
        else if (deltaX < -HalfWidth)
            camPos.x = playerPos.x + HalfWidth;

        // Vertical trap — asymmetric
        // Tighter above (follows up fast), looser below (doesn't drop immediately)
        float deltaY = playerPos.y - (camPos.y - _lookaheadOffset);
        if (deltaY > trapUp)
            camPos.y = playerPos.y - trapUp + _lookaheadOffset;
        else if (deltaY < -trapDown)
            camPos.y = playerPos.y + trapDown + _lookaheadOffset;

        // Clamp camera so it never goes below the floor
        if (useMinY)
            camPos.y = Mathf.Max(camPos.y, minY);

        camPos.z = fixedZ;
        _targetPosition = camPos;
    }

    private void ApplyPosition()
    {
        Vector3 smoothed = Vector3.Lerp(
            transform.position, _targetPosition, followSpeed * Time.deltaTime
        );

        // Round to nearest pixel to prevent sub-pixel jitter
        float pixelsPerUnit = 128f;
        smoothed.x = Mathf.Round(smoothed.x * pixelsPerUnit) / pixelsPerUnit;
        smoothed.y = Mathf.Round(smoothed.y * pixelsPerUnit) / pixelsPerUnit;

        transform.position = smoothed;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        float totalHeight = trapUp + trapDown;
        float centerOffsetY = (trapUp - trapDown) * 0.5f;

        // Filled box
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(
            new Vector3(transform.position.x, transform.position.y + centerOffsetY, 0f),
            new Vector3(trapWidth, totalHeight, 0f)
        );

        // Wire box
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, transform.position.y + centerOffsetY, 0f),
            new Vector3(trapWidth, totalHeight, 0f)
        );

        // Lookahead indicator
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - trapWidth * 0.5f, transform.position.y + _lookaheadOffset, 0f),
            new Vector3(transform.position.x + trapWidth * 0.5f, transform.position.y + _lookaheadOffset, 0f)
        );

        // Min Y indicator
        if (useMinY)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                new Vector3(transform.position.x - trapWidth * 2f, minY, 0f),
                new Vector3(transform.position.x + trapWidth * 2f, minY, 0f)
            );
        }
    }
#endif
}