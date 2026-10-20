using UnityEngine;
using System;

public class PlayerLaneMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLaneController laneController;
    [SerializeField] private PlayerLaneTargetDetection targetDetection;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.80f;
    [SerializeField] private float arcHeight = 0.8f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isMovingBetweenLanes;
    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 targetPosition;
    [SerializeField] private float transitionProgress;

    private float transitionTimer;
    private float previousGravityScale;

    public bool IsMovingBetweenLanes => isMovingBetweenLanes;
    public float TransitionProgress => transitionProgress;
    public float TransitionDuration => transitionDuration;

    public event Action OnLaneMovementStarted;
    public event Action OnLaneMovementFinished;

    private void Update()
    {
        if (isMovingBetweenLanes)
            return;

        if (inputReader == null)
            return;

        if(!inputReader.LaneSwitchRequested)
            return;

        TryStartLaneSwitch();
    }

    private void FixedUpdate()
    {
        if (!isMovingBetweenLanes)
            return;

        UpdateLaneMovement();
    }

    private void TryStartLaneSwitch()
    {
        if(inputReader == null)
            return;

        if(!inputReader.LaneSwitchRequested)
            return;

        inputReader.ConsumeLaneSwitchRequest();

        if (rb == null || laneController == null || targetDetection == null)
            return;

        if (laneController.IsTransitioning)
            return;

        if (playerMovement != null)
        {
            if (!playerMovement.IsGrounded)
                return;

            if (playerMovement.IsCrouching)
                return;

            if (playerMovement.IsDead())
                return;
        }

        GameplayLane targetLane = laneController.GetOppositeLane();

        if (!targetDetection.TryFindTarget(targetLane, out Vector2 detectedTargetPosition))
        {
            Debug.Log($"[LANE MOVE] Nenhum destino válido em {targetLane}", this);

            return;
        }

        if (!laneController.BeginTransition(targetLane))
            return;

        StartLaneMovement(detectedTargetPosition);
    }

    private void StartLaneMovement(Vector2 newTargetPosition)
    {
        startPosition = rb.position;
        targetPosition = newTargetPosition;

        transitionTimer = 0f;
        transitionProgress = 0f;

        previousGravityScale = rb.gravityScale;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        isMovingBetweenLanes = true;

        OnLaneMovementStarted?.Invoke();

        if(playerMovement != null)
            playerMovement.SetLaneTransitionActive(true);

        Debug.Log($"[LANE MOVE] Início: {startPosition} -> {targetPosition}", this);
    }

    private void UpdateLaneMovement()
    {
        if(rb == null)
            return;

        float duration = Mathf.Max(0.01f, transitionDuration);

        transitionTimer += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(transitionTimer / duration);

        transitionProgress = t;

        Vector2 basePosition = Vector2.Lerp(startPosition, targetPosition, t);

        float arcOffset = 4f * arcHeight * t * (1f -t);

        Vector2 finalPosition = basePosition + Vector2.up * arcOffset;

        rb.MovePosition(finalPosition);

        if(t >= 1f)
            FinishLaneMovement();

    }

    private void FinishLaneMovement()
    {
        rb.position = targetPosition;
        rb.linearVelocity = Vector2.zero;

        laneController.CompleteTransition();

        rb.gravityScale = previousGravityScale;

        isMovingBetweenLanes = false;
        transitionProgress = 1f;

        if(playerMovement != null)
            playerMovement.SetLaneTransitionActive(false);

        OnLaneMovementFinished?.Invoke();

        Debug.Log($"[LANE MOVE] Concluído. Lane atual: {laneController.CurrentLane}", this);
    }

}
