using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private CameraTargetController cameraTargetController;
    [SerializeField] private PlayerCombat playerCombat;

    [Header("Body Collider")]
    [SerializeField] private CapsuleCollider2D bodyCollider;
    [SerializeField] private Vector2 standingColliderSize;
    [SerializeField] private Vector2 standingColliderOffset;
    [SerializeField] private Vector2 crouchingColliderSize;
    [SerializeField] private Vector2 crouchingColliderOffset;

    [Header("Hurtbox")]
    [SerializeField] private BoxCollider2D hurtboxCollider;
    [SerializeField] private Vector2 standingHurtboxSize;
    [SerializeField] private Vector2 standingHurtboxOffset;
    [SerializeField] private Vector2 crouchingHurtboxSize;
    [SerializeField] private Vector2 crouchingHurtboxOffset;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpImpulse = 12f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Vertical State Debug")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGroundedLastFrame;
    [SerializeField] private bool jumpStartedThisFrame;
    [SerializeField] private bool landedThisFrame;
    [SerializeField] private bool leftGroundThisFrame;

    private float moveInputX;
    private bool isFacingRight = true;

    [Header("Crouch State")]
    [SerializeField]private bool isCrouching;
    [SerializeField] private bool enteredCrouchThisFrame;
    [SerializeField] private bool wasCrouchingLastFrame;

    public bool IsMovingHorizontally => Mathf.Abs(moveInputX) > 0.01f;
    public bool IsFacingRight => isFacingRight;
    public bool IsGrounded => isGrounded;
    public float VerticalVelocity => rb != null ? rb.linearVelocity.y : 0f;
    public bool IsRising => VerticalVelocity > 0.01f;
    public bool IsFalling => VerticalVelocity < 0.01f;
    public bool JumpStartedThisFrame => jumpStartedThisFrame;
    public bool LandedThisFrame => landedThisFrame;
    public bool HasJumpRequest => inputReader != null && inputReader.JumpRequested;
    public bool LeftGroundThisFrame => leftGroundThisFrame;
    public bool IsAirborne => !isGrounded;
    public bool IsCrouching => isCrouching;

    private void Start()
    {
        ApplyStandingBodyCollider();
        ApplyStandingHurtbox();
        wasCrouchingLastFrame = false;
    }

    private void Update()
    {
        if (inputReader == null)
        {
            moveInputX = 0f;
          
            return;
        }

        moveInputX = inputReader.MoveInputX;
 
        UpdateCrouchState();
        UpdateBodyColliderForCrouch();
        UpdateHurtboxForCrouch();
        HandleFacingDirection();
    }

    private void FixedUpdate()
    {
        ResetFrameFlags();
        CheckGround();

        if (rb == null)
            return;

        if (enteredCrouchThisFrame)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        HandleHorizontalMovement();
        HandleJump();

    }

    private float GetFilteredMoveInputX()
    {
        float filteredMoveInputX = moveInputX;

        if (cameraTargetController != null && cameraTargetController.IsBlockingBackwardMovement && filteredMoveInputX < 0f)
        {
            filteredMoveInputX = 0f;
        }

        return filteredMoveInputX;
    }

    private void HandleHorizontalMovement()
    {
        if (playerCombat != null && playerCombat.IsAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        if (isCrouching)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float filteredMoveInputX = GetFilteredMoveInputX();
        rb.linearVelocity = new Vector2(filteredMoveInputX * moveSpeed, rb.linearVelocity.y);
    }

    private void ResetFrameFlags()
    {
        jumpStartedThisFrame = false;
        landedThisFrame = false;
        leftGroundThisFrame = false;
        enteredCrouchThisFrame = false;
    }

    private void CheckGround()
    {
        wasGroundedLastFrame = isGrounded;

        if (groundCheck == null)
        {
            isGrounded = false;
            landedThisFrame = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        leftGroundThisFrame = wasGroundedLastFrame && !isGrounded;
        landedThisFrame = !wasGroundedLastFrame && isGrounded;
    }

    void HandleFacingDirection()
    {
        if (playerCombat != null && playerCombat.IsAttacking)
            return;

        if (isCrouching)
            return;

        if(moveInputX > 0f && !isFacingRight)
        {
            Flip(true);
        }

        else if(moveInputX < 0f && isFacingRight)
        {
            Flip(false);
        }
    }

    void Flip(bool faceRight)
    {
        isFacingRight = faceRight;

        if (visual == null)
            return;

        Vector3 scale = visual.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        visual.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void HandleJump()
    {
        if (inputReader == null || rb == null)
            return;

        if (!inputReader.JumpRequested)
            return;

        if (isCrouching)
        {
            inputReader.ConsumeJumpRequest();
            return;
        }
            
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

            jumpStartedThisFrame = true;
        }

            inputReader.ConsumeJumpRequest();
    }

    private bool CanEnterOrStayCrouching()
    {
        if (inputReader == null)
            return false;

        if (!inputReader.IsCrouchHeld)
            return false;

        if (!isGrounded)
            return false;

        if (playerCombat != null && playerCombat.IsAttacking)
            return false;

        return true;
    }

    private void UpdateCrouchState()
    {
        bool wasCrouching = isCrouching;
        isCrouching = CanEnterOrStayCrouching();
        enteredCrouchThisFrame = !wasCrouching && isCrouching;

    }

    private void ApplyStandingBodyCollider()
    {
        if (bodyCollider == null)
            return;

        bodyCollider.size = standingColliderSize;
        bodyCollider.offset = standingColliderOffset;
    }

    private void ApplyCrouchingBodyCollider()
    {
        if (bodyCollider == null)
            return;

        bodyCollider.size = crouchingColliderSize;
        bodyCollider.offset = crouchingColliderOffset;
    }

    private void ApplyStandingHurtbox()
    {
        if (hurtboxCollider == null)
            return;

        hurtboxCollider.size = standingHurtboxSize;
        hurtboxCollider.offset = standingHurtboxOffset;
    }

    private void ApplyCrouchingHurtbox()
    {
        if (hurtboxCollider == null)
            return;

        hurtboxCollider.size = crouchingHurtboxSize;
        hurtboxCollider.offset = crouchingHurtboxOffset;
    }

    private void UpdateBodyColliderForCrouch()
    {
        if (bodyCollider == null)
            return;

        if (isCrouching == wasCrouchingLastFrame)
            return;

        if (isCrouching)
            ApplyCrouchingBodyCollider();

        else
            ApplyStandingBodyCollider();

        wasCrouchingLastFrame = isCrouching;
    }

    private void UpdateHurtboxForCrouch()
    {
        if (hurtboxCollider == null)
            return;

        if (isCrouching)
            ApplyCrouchingHurtbox();

        else
            ApplyStandingHurtbox();

        wasCrouchingLastFrame = isCrouching;
    }
}
