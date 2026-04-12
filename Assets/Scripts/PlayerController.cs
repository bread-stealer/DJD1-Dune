using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerShield))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private PlayerShield _shield;
    private PlayerHealth _health;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _shield = GetComponent<PlayerShield>();
        _health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        _rb.linearVelocity = new Vector2(horizontal * moveSpeed, _rb.linearVelocity.y);

        if (horizontal != 0f)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1f, 1f);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x , jumpForce);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void TakeDamage(AttackData attack)
    {
        // Blocked by the shield
        if (_shield != null && _shield.TryBlock(attack))
            return;

        // Feed into Health System
        if (_health != null)
            _health.TakeDamage(attack.Damage);

        // Player health system, to expand later
        Debug.Log($"Player took {attack.Damage} damage");
    }

    // An instruction to the compiler itself, processed before the code is compiled
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif

}
