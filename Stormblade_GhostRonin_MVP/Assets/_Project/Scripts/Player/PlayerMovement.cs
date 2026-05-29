using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Transform visual;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;

    private float moveInputX;
    private float jumpInput;
    private bool isFacingRight = true;

    public bool IsFacingRight => isFacingRight;
    public bool IsMovingHorizontally => Mathf.Abs(moveInputX) > 0.01f;

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

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);


    }

    
}
