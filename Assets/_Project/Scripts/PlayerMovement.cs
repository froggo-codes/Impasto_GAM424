using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")] [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private AudioClip jumpSound;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Jump")] [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private float wallJumpPushSpeed = 8f;
    [SerializeField] private float wallJumpLockTime = 0.2f;

    [Header("Dash")] 
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.4f;
    [SerializeField] private AudioClip dashSound;

    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool touchingWallLeft;
    private bool touchingWallRight;
    private float wallJumpLockTimer;
    private AudioSource audioSource;

    private float facingDirection = 1f;
    private bool dashPressed;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool canAirDash = true;
    private float normalGravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        normalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        moveInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput += 1f;

        if (moveInput != 0f)
            facingDirection = moveInput;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            dashPressed = true;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
        touchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer) != null;
        touchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundLayer) != null;

        if (isGrounded)
            canAirDash = true;

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        if (dashPressed && !isDashing && dashCooldownTimer <= 0f && (isGrounded || canAirDash))
            StartDash();

        dashPressed = false;

        if (isDashing)
        {
            rb.linearVelocity = new Vector2(facingDirection * dashSpeed, 0f);
            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0f)
                EndDash();

            jumpPressed = false;
            return;
        }

        if (wallJumpLockTimer > 0f)
            wallJumpLockTimer -= Time.fixedDeltaTime;
        else
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (jumpPressed)
        {
            float jumpVelocity = Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight);

            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
                audioSource.PlayOneShot(jumpSound);
            }
            else if (touchingWallLeft || touchingWallRight)
            {
                float pushDirection = touchingWallLeft ? 1f : -1f;
                rb.linearVelocity = new Vector2(pushDirection * wallJumpPushSpeed, jumpVelocity);
                wallJumpLockTimer = wallJumpLockTime;
                audioSource.PlayOneShot(jumpSound);
            }
        }

        jumpPressed = false;
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        wallJumpLockTimer = 0f;

        if (!isGrounded)
            canAirDash = false;

        rb.gravityScale = 0f;

        if (dashSound != null)
            audioSource.PlayOneShot(dashSound);
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = normalGravityScale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.cyan;
        if (wallCheckLeft != null)
            Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);
        if (wallCheckRight != null)
            Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);
    }
}