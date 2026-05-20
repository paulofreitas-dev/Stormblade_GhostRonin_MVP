using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;

    private float moveInputX;

    private void Update()
    {
        if (inputReader == null)
        {
            moveInputX = 0f;
            return;
        }

        moveInputX = inputReader.MoveInputX;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);

    }
}
