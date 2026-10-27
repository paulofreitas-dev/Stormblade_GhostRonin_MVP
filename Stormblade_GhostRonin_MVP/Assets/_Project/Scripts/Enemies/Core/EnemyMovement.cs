using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Runtime Debug")]
    [SerializeField] private float currentDirection;
    [SerializeField] private float lastNonZeroDirection;
    [SerializeField] private bool movementEnabled = true;

    public float CurrentDirection => currentDirection;
    public float LastNonZeroDirection => lastNonZeroDirection;
    public float MoveSpeed => moveSpeed;
    public bool MovementEnabled => movementEnabled;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void ApplyHorizontalMovement()
    {
        if(rb == null)
            return;

        float targetVelocityX = 0f;

        if(movementEnabled)
            targetVelocityX = currentDirection * moveSpeed;

        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }

    public void SetMoveDirection(float direction)
    {
        if(Mathf.Abs(direction) < 0.01f)
        {
            currentDirection = 0f;
            return;
        }

        currentDirection = Mathf.Sign(direction);
        lastNonZeroDirection = currentDirection;
    }

    public void Stop()
    {
        currentDirection = 0f;

        if(rb == null)
            return;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if(!movementEnabled)
            Stop();
    }
}
