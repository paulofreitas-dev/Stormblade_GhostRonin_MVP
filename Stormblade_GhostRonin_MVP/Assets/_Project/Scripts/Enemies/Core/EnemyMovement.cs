using Unity.VisualScripting;
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

    [Header("Damage Pushback State")]
    [SerializeField] private bool isDamagePushbackActive;
    [SerializeField] private float damagePushbackTimer;

    private float damagePushbackVelocityX;

    public float CurrentDirection => currentDirection;
    public float LastNonZeroDirection => lastNonZeroDirection;
    public float MoveSpeed => moveSpeed;
    public bool MovementEnabled => movementEnabled;
    public bool IsDamagePushbackActive => isDamagePushbackActive;

    private void Awake()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // O pushback tem prioridade sobre o avanço normal
        if(HandleDamagePushback())
            return;

        ApplyHorizontalMovement();
    }

    private void OnDisable()
    {
        // Evita retomar um pushback antigo ao reabilitar o componente após uma transição de lane
        CancelDamagePushback();
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

        // Parar um avanço convencional não deve interromper um pushback em andamento
        if(isDamagePushbackActive)
            return;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!movementEnabled)
        {
            CancelDamagePushback();
            Stop();
        }
    }

    public void StartDamagePushback(float directionX, float speed, float duration)
    {
        if(rb == null || !isActiveAndEnabled)
            return;

        if(Mathf.Abs(directionX) < 0.01f)
            return;

        speed = Mathf.Max(0f, speed);
        duration = Mathf.Max(0f, duration);

        if(speed <= 0f || duration <= 0f)
            return;

        damagePushbackVelocityX = Mathf.Sign(directionX) * speed;

        damagePushbackTimer = duration;
        isDamagePushbackActive = true;

        rb.linearVelocity = new Vector2(damagePushbackVelocityX, rb.linearVelocity.y);
    }

    private bool HandleDamagePushback()
    {
        if(!isDamagePushbackActive)
            return false;

        if(rb == null)
        {
            CancelDamagePushback();
            return false;
        }

        rb.linearVelocity = new Vector2(damagePushbackVelocityX, rb.linearVelocity.y);

        damagePushbackTimer -= Time.fixedDeltaTime;

        if(damagePushbackTimer <= 0f)
        {
            isDamagePushbackActive = false;
            damagePushbackTimer = 0f;
            damagePushbackVelocityX = 0f;
        }

        return true;
    }

    public void CancelDamagePushback()
    {
        if(!isDamagePushbackActive)
            return;

        isDamagePushbackActive = false;
        damagePushbackTimer = 0f;
        damagePushbackVelocityX = 0f;

        if(rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}
