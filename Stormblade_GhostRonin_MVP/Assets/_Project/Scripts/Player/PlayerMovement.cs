using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private CameraTargetController cameraTargetController;

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

    private void Update()
    {
        if (inputReader == null)
        {
            moveInputX = 0f;
          
            return;
        }

        moveInputX = inputReader.MoveInputX;
 
        HandleFacingDirection();

    }

    private void FixedUpdate()
    {
        ResetFrameFlags();
        CheckGround();

        if (rb == null)
            return;

        HandleHorizontalMovement();
        HandleJump();

        Debug.Log($"Grounded: {isGrounded} | Rising: {IsRising} | Falling: {IsFalling} | Y Vel: {VerticalVelocity}");

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
        float filteredMoveInputX = GetFilteredMoveInputX();
        rb.linearVelocity = new Vector2(filteredMoveInputX * moveSpeed, rb.linearVelocity.y);
    }

    private void ResetFrameFlags()
    {
        jumpStartedThisFrame = false;
        landedThisFrame = false;
        leftGroundThisFrame = false;
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

        //if (landedThisFrame)
        //{
        //    Debug.Log("Acabou de aterrissar");
        //}
    }

    void HandleFacingDirection()
    {
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

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

            jumpStartedThisFrame = true;

            //Debug.Log("Pulo executado.");
        }

        //else
        //{
        //    Debug.Log("Pedido de pulo descartado: o player não está grounded");
        //}

            inputReader.ConsumeJumpRequest();
    }
}
