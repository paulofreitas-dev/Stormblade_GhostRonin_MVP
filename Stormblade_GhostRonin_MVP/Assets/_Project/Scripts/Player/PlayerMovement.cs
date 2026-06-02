using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform groundCheck;


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



    private void Update()
    {
        if (inputReader == null)
        {
            moveInputX = 0f;
          
            return;
        }

        moveInputX = inputReader.MoveInputX;
 

        HandleFacingDirection();

        if (landedThisFrame)
        {
            Debug.Log("Acabou de aterrissar");
        }
    }

    private void FixedUpdate()
    {
        ResetFrameFlags();
        CheckGround();

        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
    }

    private void ResetFrameFlags()
    {
        jumpStartedThisFrame = false;
        landedThisFrame = false;
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

        landedThisFrame = !wasGroundedLastFrame && isGrounded;

        if (landedThisFrame)
        {
            Debug.Log("Acabou de aterrissar");
        }
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
}
