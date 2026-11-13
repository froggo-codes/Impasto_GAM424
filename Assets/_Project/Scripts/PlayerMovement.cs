using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 6f;
    
    [Header("Jump")] [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private AudioClip jumpSound;

    [Header("Ground Check")] 
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float safeGroundMargin = 0.6f;

    [Header("Wall Jump")] [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private float wallJumpPushSpeed = 8f;
    [SerializeField] private float wallJumpLockTime = 0.2f;

    [Header("Dash")] [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.4f;
    [SerializeField] private AudioClip dashSound;

    [Header("Climbing")] 
    [SerializeField] private float climbSpeed = 4f;
    [SerializeField] private float regrabDelay = 0.25f;

    [Header("Damage")]
    [SerializeField] private Vector2 knockbackVelocity = new Vector2(6f, 8f);
    [SerializeField] private float knockbackLockTime = 0.3f;
    
    [Header("Piano")]
    [SerializeField] private GameObject pianoUI;

private Rigidbody2D rb;
    private float moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private bool touchingWallLeft;
    private bool touchingWallRight;
    private float inputLockTimer;
    private AudioSource audioSource;

    private float facingDirection = 1f;
    private bool dashPressed;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private bool canAirDash = true;
    private float normalGravityScale;

    private float climbInput;
    private bool isClimbing;
    private Rope currentRope;
    private float regrabTimer;
    private Vector2 lastSafePosition;

    private MovingPlatform currentPlatform; // the moving platform player is currently on

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        lastSafePosition = rb.position;
        audioSource = GetComponent<AudioSource>();
        normalGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (PauseController.IsGamePaused) return;
        
        moveInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput -= 1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput += 1f;

        if (moveInput != 0f)
            facingDirection = moveInput;

        climbInput = 0f;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            climbInput += 1f;

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            climbInput -= 1f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            dashPressed = true;
        
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            PlayPiano();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) != null;
        touchingWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, groundLayer) != null;
        touchingWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, groundLayer) != null;

        if (isGrounded)
        {
            canAirDash = true;

            Vector2 checkPos = groundCheck.position;
            bool groundLeft = Physics2D.OverlapCircle(checkPos + Vector2.left * safeGroundMargin, groundCheckRadius, groundLayer) !=null;
            bool groundRight = Physics2D.OverlapCircle(checkPos + Vector2.right * safeGroundMargin, groundCheckRadius, groundLayer) != null;

            if (groundLeft && groundRight)
                lastSafePosition = rb.position;
        }

        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        if (regrabTimer > 0f)
            regrabTimer -= Time.fixedDeltaTime;

        if (!isClimbing && !isDashing && currentRope != null && climbInput > 0f && regrabTimer <= 0f)
            StartClimb();

        if (isClimbing)
        {
            if (currentRope == null || (isGrounded && climbInput < 0f))
            {
                StopClimb();
            }
            else
            {
                if (jumpPressed)
                {
                    StopClimb();
                    rb.linearVelocity = new Vector2(moveInput * moveSpeed, GetJumpVelocity());
                    regrabTimer = regrabDelay;
                    audioSource.PlayOneShot(jumpSound);
                }
                else
                {
                    rb.position = new Vector2(currentRope.transform.position.x, rb.position.y);
                    rb.linearVelocity = new Vector2(0f, climbInput * climbSpeed);
                }

                jumpPressed = false;
                dashPressed = false;
                return;
            }
        }

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

        float platformSpeedX = currentPlatform != null ? currentPlatform.Velocity.x : 0f;
        
        if (inputLockTimer > 0f)
            inputLockTimer -= Time.fixedDeltaTime;
        else
            rb.linearVelocity = new Vector2(moveInput * moveSpeed + platformSpeedX, rb.linearVelocity.y);

        if (jumpPressed)
        {
            float jumpVelocity = GetJumpVelocity();

            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
                audioSource.PlayOneShot(jumpSound);
            }
            else if (touchingWallLeft || touchingWallRight)
            {
                float pushDirection = touchingWallLeft ? 1f : -1f;
                rb.linearVelocity = new Vector2(pushDirection * wallJumpPushSpeed, jumpVelocity);
                inputLockTimer = wallJumpLockTime;
                audioSource.PlayOneShot(jumpSound);
            }
        }

        jumpPressed = false;
    }

    float GetJumpVelocity()
    {
        return Mathf.Sqrt(2f * Physics2D.gravity.magnitude * rb.gravityScale * jumpHeight);
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        inputLockTimer = 0f;

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

    public void Respawn()
    {
        if (isDashing) EndDash();
        if (isClimbing) StopClimb();

        rb.linearVelocity = Vector2.zero;
        transform.position = lastSafePosition + Vector2.up * 0.5f;
    }
    
    void StartClimb()
    {
        isClimbing = true;
        inputLockTimer = 0f;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    void StopClimb()
    {
        isClimbing = false;
        rb.gravityScale = normalGravityScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Rope rope = other.GetComponent<Rope>();
        if (rope != null)
            currentRope = rope;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Rope rope = other.GetComponent<Rope>();
        if (rope != null && rope == currentRope)
            currentRope = null;
    }

    public void Knockback(Vector2 sourcePosition)
    {
        if (isDashing) EndDash();
        if (isClimbing) StopClimb();

        float direction = transform.position.x < sourcePosition.x ? -1f : 1f;
        rb.linearVelocity = new Vector2(direction * knockbackVelocity.x, knockbackVelocity.y);
        inputLockTimer = knockbackLockTime;
    }
    
    public void Bounce(float bounceVelocity)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceVelocity);
        canAirDash = true;
    }

    public void SetPlatform(MovingPlatform platform)
    {
        currentPlatform = platform;
    }

    public void ClearPlatform(MovingPlatform platform)
    {
        // Only clear if it's the SAME platform we're on.
        // (if we hop from platform A to B, A's "exit" can arrive after B's "enter")
        if (currentPlatform == platform) currentPlatform = null;
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

    public void PlayPiano()
    {
        if (!pianoUI.activeInHierarchy)
        {
            pianoUI.SetActive(true);

        }
        else
        {
            pianoUI.SetActive(false);
        }
    }
}