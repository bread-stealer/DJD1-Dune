using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
     [Header("Movement")]
     [SerializeField] private float moveSpeed = 8f;
      
     [Header("Jump")]
     [SerializeField] private float jumpForce = 16f;
     [SerializeField] private float fallMultiplier = 2.5f;
     [SerializeField] private float lowJumpMultiplier = 2f;

     [Header("Ground Detection")]
     [SerializeField] private Transform     groundCheck;
     [SerializeField] private float         groundCheckRadius = 0.15f;
     [SerializeField] private LayerMask groundLayer;
        
    private void Start()
    {

    }

    private void Update()
    {
        
    }
}
