using UnityEngine;
using System;
using Unity.VisualScripting;

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
        if (inputReader == null)
            return;

        // Não acumula comandos durante uma transição
        if (isMovingBetweenLanes)
        {
            inputReader.ConsumeLaneBackRequest();
            inputReader.ConsumeLaneFrontRequest();
            inputReader.ConsumeLaneSwitchRequest();
            return;
        }

        bool backRequested = inputReader.LaneBackRequested;
        bool frontRequested = inputReader.LaneFrontRequested;
        bool switchRequested = inputReader.LaneSwitchRequested;

        if (!backRequested && !frontRequested && !switchRequested)
            return;

        // Consome os pedidos antes de validar o destino
        inputReader.ConsumeLaneBackRequest();
        inputReader.ConsumeLaneFrontRequest();
        inputReader.ConsumeLaneSwitchRequest();

        // Dois destinos opostos simultâneos: ignora
        if (backRequested && frontRequested)
            return;

        if (laneController == null)
            return;

        GameplayLane requestedLane;

        if (backRequested)
            requestedLane = GameplayLane.Back;
        else if (frontRequested)
            requestedLane = GameplayLane.Front;
        else
            requestedLane = laneController.GetOppositeLane();

        TryStartLaneSwitch(requestedLane);
    }

    private void FixedUpdate()
    {
        if (!isMovingBetweenLanes)
            return;

        UpdateLaneMovement();
    }

    private void TryStartLaneSwitch(GameplayLane requestedLane)
    {
        if (rb == null || laneController == null || targetDetection == null)
            return;

        if (laneController.IsTransitioning)
            return;

        // Já estamos na lane solicitada
        if(laneController.CurrentLane == requestedLane)
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

        if (!targetDetection.TryFindTarget(requestedLane, out Vector2 detectedTargetPosition))
        {
            Debug.Log($"[LANE MOVE] Nenhum destino válido em {requestedLane}", this);

            return;
        }

        if (!laneController.BeginTransition(requestedLane))
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

        if (playerMovement != null)
            playerMovement.SetLaneTransitionActive(true);

        Debug.Log($"[LANE MOVE] Início: {startPosition} -> {targetPosition}", this);
    }

    private void UpdateLaneMovement()
    {
        if (rb == null)
            return;

        float duration = Mathf.Max(0.01f, transitionDuration);

        transitionTimer += Time.fixedDeltaTime;

        float t = Mathf.Clamp01(transitionTimer / duration);

        transitionProgress = t;

        Vector2 basePosition = Vector2.Lerp(startPosition, targetPosition, t);

        float arcOffset = 4f * arcHeight * t * (1f - t);

        Vector2 finalPosition = basePosition + Vector2.up * arcOffset;

        rb.MovePosition(finalPosition);

        if (t >= 1f)
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

        if (playerMovement != null)
            playerMovement.SetLaneTransitionActive(false);

        OnLaneMovementFinished?.Invoke();

        Debug.Log($"[LANE MOVE] Concluído. Lane atual: {laneController.CurrentLane}", this);
    }

}
