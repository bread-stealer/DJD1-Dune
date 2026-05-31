using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerShield))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] [Range(0f, 1f)] private float airControlFactor = 0.7f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private Animator _animator;
    private PlayerShield _shield;
    private PlayerHealth _health;

    private bool _isJumping;
    private bool _wasGrounded;

    // Cached animator parameter hashes
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _animator = GetComponentInChildren<Animator>();
        _shield = GetComponent<PlayerShield>();
        _health = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        HandleMove();
        HandleJump();
        HandleJumpCut();
        UpdateAnimations();
    }

    private void HandleMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float speed = IsGrounded() ? moveSpeed : moveSpeed * airControlFactor;
        _rb.linearVelocity = new Vector2(horizontal * speed, _rb.linearVelocity.y);

        if (horizontal != 0f)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1f, 1f);
    }

    private void HandleJump()
    {
        bool grounded = IsGrounded();

        if (Input.GetButtonDown("Jump") && grounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _isJumping = true;
        }

        if (grounded && !_wasGrounded)
            _isJumping = false;

        _wasGrounded = grounded;
    }

    // If the player releases Jump while still rising, cut the velocity for a short hop
    private void HandleJumpCut()
    {
        if (Input.GetButtonUp("Jump") && _isJumping && _rb.linearVelocity.y > 0f)
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * jumpCutMultiplier);
    }

    private void UpdateAnimations()
    {
        _animator.SetFloat(SpeedHash, Mathf.Abs(_rb.linearVelocity.x));
        _animator.SetBool(IsGroundedHash, IsGrounded());
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void TakeDamage(AttackData attack)
    {
        if (_shield != null && _shield.TryBlock(attack))
            return;

        if (_health != null)
            _health.TakeDamage(attack.Damage);

        Debug.Log($"Player took {attack.Damage} damage");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
