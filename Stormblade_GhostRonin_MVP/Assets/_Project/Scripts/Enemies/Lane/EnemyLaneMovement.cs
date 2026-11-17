using UnityEngine;
using System;

public class EnemyLaneMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyLaneController laneController;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.35f;
    [SerializeField] private float arcHeight = 0.8f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isMovingBetweenLanes;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float transitionProgress;

    private float transitionTimer;
    private float previousGravityScale;
    private bool previousMovementEnabled;

    public bool IsMovingBetweenLanes => isMovingBetweenLanes;
    public float TransitionProgress => transitionProgress;

    public event Action OnLaneMovementStarted;
    public event Action OnLaneMovementFinished;
    public event Action OnLaneMovementCancelled;

    private void FixedUpdate()
    {
        if(!isMovingBetweenLanes)
            return;

        UpdateLaneMovement();
    }

    public bool StartMovement(Vector2 destination)
    {
        if(isMovingBetweenLanes)
            return false;

        if(rb == null || laneController == null)
            return false;

        if(!laneController.IsTransitioning)
            return false;

        startPosition = rb.position;
        targetPosition = destination;

        transitionTimer = 0f;
        transitionProgress = 0f;

        //Suspende temporariamente a movimentação normal.
        if(enemyMovement != null)
        {
            previousMovementEnabled = enemyMovement.enabled;
            enemyMovement.enabled = false;
        }

        previousGravityScale = rb.gravityScale;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        isMovingBetweenLanes = true;

        OnLaneMovementStarted?.Invoke();

        Debug.Log($"[ENEMY LANE MOVE] Início: " + 
        $"{startPosition} -> {targetPosition}",
        this);

        return true;
    }

    private void UpdateLaneMovement()
    {
        float duration = Mathf.Max(0.01f, transitionDuration);

        transitionTimer += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(transitionTimer / duration);

        transitionProgress = t;

        Vector2 basePosition = Vector2.Lerp(startPosition, targetPosition, t);

        float arcOffset = 4f * arcHeight * t * (1f - t);

        Vector2 finalPosition = basePosition + Vector2.up * arcOffset;

        rb.MovePosition(finalPosition);

        if(t >= 1f)
            FinishMovement();
    }

    private void FinishMovement()
    {
        rb.position = targetPosition;
        rb.linearVelocity = Vector2.zero;

        //Confirma a nova lane somente após o deslocamento.
        laneController.CompleteTransition();

        RestorePhysics();

        isMovingBetweenLanes = false;
        transitionProgress = 1f;

        OnLaneMovementFinished?.Invoke();

        Debug.Log($"[ENEMY LANE MOVE] Concluído. " +
        $"Lane atual: {laneController.CurrentLane}",
        this);
    }

    public void CancelMovement()
    {
        if(!isMovingBetweenLanes)
            return;

        rb.position = startPosition;
        rb.linearVelocity = Vector2.zero;

        laneController.CancelTransition();

        RestorePhysics();

        isMovingBetweenLanes = false;
        transitionProgress = 0f;

        OnLaneMovementCancelled?.Invoke();

        Debug.Log("[ENEMY LANE MOVE] Transição cancelada.",
        this);
    }

    private void RestorePhysics()
    {
        rb.gravityScale = previousGravityScale;

        if(enemyMovement != null)
            enemyMovement.enabled = previousMovementEnabled;
    }
}
