using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Trap Box Size")]
    [SerializeField] private float trapWidth = 4f;
    [SerializeField] private float trapHeight = 3f;

    [Header("Camera Position")]
    [SerializeField] private float fixedZ = -10f;

    [Header("Follow Speed")]
    [SerializeField, Min(0f)] private float followSpeed = 8f;

    private Vector3 _targetPosition;

    private float HalfWidth => trapWidth * 0.5f;
    private float HalfHeight => trapHeight * 0.5f;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("[CameraSystem] Player Transform is not assigned.", this);
            enabled = false;
            return;
        }
    }
    private void LateUpdate()
    {
        UpdateTrap();
        ApplyPosition();
    }

    private void UpdateTrap()
    {
        Vector3 camPos = _targetPosition;
        Vector3 playerPos = player.position;

        float deltaX = playerPos.x - camPos.x;
        if (deltaX > HalfWidth)
            camPos.x = playerPos.x - HalfWidth;
        else if (deltaX < -HalfWidth)
            camPos.x = playerPos.x + HalfWidth;

        float deltaY = playerPos.y - camPos.y;
        if (deltaY > HalfHeight)
            camPos.y = playerPos.y - HalfHeight;
        else if (deltaY < -HalfHeight)
            camPos.y = playerPos.y + HalfHeight;

        camPos.z = fixedZ;
        _targetPosition = camPos;
    }

    private void ApplyPosition()
    {
        transform.position = Vector3.Lerp(
            transform.position, _targetPosition, followSpeed * Time.deltaTime
            );
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color (0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y, 0f),
                        new Vector3(trapWidth, trapHeight, 0f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, 0f),
                        new Vector3(trapWidth, trapHeight, 0f));
    }
#endif
}
