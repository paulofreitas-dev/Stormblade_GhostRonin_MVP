using UnityEngine;

public class OniSoldierBrain : MonoBehaviour, IEnemySpawnDirectionConfigurable
{
    [Header("References")]
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private EnemyTargetDetector targetDetector;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyFacingController facingController;
    [SerializeField] private EnemyLaneMovement laneMovement;

    [Header("Advance Settings")]
    [SerializeField] private float advanceDirection = -1f;

    [Header("Step Advance")]
    [SerializeField] private float pauseDuration = 0.15f;
    [SerializeField] private float stepDuration = 0.10f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isAdvancing;
    [SerializeField] private bool isStepMoving;
    [SerializeField] private float advanceTimer;
    [SerializeField] private bool resumeAdvanceAfterLaneTransition;

    public bool IsAdvancing => isAdvancing;

    public void Awake()
    {
        if(movement == null)
            movement = GetComponent<EnemyMovement>();

        if(animationController == null)
            animationController = GetComponent<EnemyAnimationController>();

        if(targetDetector == null)
            targetDetector = GetComponent<EnemyTargetDetector>();

        if(combat == null)
            combat = GetComponent<EnemyCombat>();

        if(facingController == null)
            facingController = GetComponent<EnemyFacingController>();

        if(laneMovement == null)
            laneMovement = GetComponent<EnemyLaneMovement>();

        // ApplyAdvanceDirection();
    }

    private void Start()
    {
        if(combat != null)
            combat.OnAttackFinished += HandleAttackFinished;

        if(laneMovement != null)
        {
            laneMovement.OnLaneMovementStarted += HandleLaneMovementStarted;

            laneMovement.OnLaneMovementFinished += HandleLaneMovementEnded;

            laneMovement.OnLaneMovementCancelled += HandleLaneMovementEnded;
        }

        BeginAdvance();
    }

    private void OnDestroy()
    {
        if(combat != null)
            combat.OnAttackFinished -= HandleAttackFinished;

        if(laneMovement != null)
        {
            laneMovement.OnLaneMovementStarted -= HandleLaneMovementStarted;

            laneMovement.OnLaneMovementFinished -= HandleLaneMovementEnded;

            laneMovement.OnLaneMovementCancelled -= HandleLaneMovementEnded;
        }
    }

    private void Update()
    {
        // Bloqueia a IA durante a troca de lane
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        if(!isAdvancing)
            return;

        if(TryBeginAttack())
            return;

        UpdateAdvanceCycle();
    }

    public void BeginAdvance()
    {
        // Se estiver saltando, adia a retomada
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
        {
            resumeAdvanceAfterLaneTransition = true;

            return;
        }
  
        if(movement == null)
            return;

        advanceDirection = advanceDirection >= 0f ? 1f : -1f;

        isAdvancing = true;

        movement.SetMovementEnabled(true);

        if(facingController != null)
            facingController.SetFacing(advanceDirection);

        BeginPausePhase();
    }

    public void StopAdvance()
    {
        isAdvancing = false;
        isStepMoving = false;
        advanceTimer = 0f;

        if(movement != null)
            movement.Stop();

        if(animationController != null)
            animationController.SetMoving(false);
    }

    private void BeginAttack()
    {
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        StopAdvance();

        if(!combat.TryStartAttack())
            BeginAdvance();
    }

    private void HandleAttackFinished()
    {
        BeginAdvance();
    }

    private void HandleLaneMovementStarted()
    {
        if(!isActiveAndEnabled)
            return;

        //Guarda a intenção de avançar
        resumeAdvanceAfterLaneTransition = isAdvancing || resumeAdvanceAfterLaneTransition;

        //Interrompe avanço e ciclo de passos
        StopAdvance();
    }

    private void HandleLaneMovementEnded()
    {
        if(!isActiveAndEnabled)
            return;

        // Outra transição pode já ter começado
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        bool shouldResume = resumeAdvanceAfterLaneTransition;

        resumeAdvanceAfterLaneTransition = false;

        if(shouldResume)
            BeginAdvance();
    }

    public void SetInitialDirection(float direction)
    {
        if(Mathf.Approximately(direction, 0f))
            return;

        advanceDirection = Mathf.Sign(direction);

        ApplyAdvanceDirection();
    }

    private void ApplyAdvanceDirection()
    {
        advanceDirection = advanceDirection >= 0f ? 1f : -1f;

        if(movement != null)
            movement.SetMoveDirection(advanceDirection);

        if(facingController != null)
            facingController.SetFacing(advanceDirection);
    }

    private void UpdateAdvanceCycle()
    {
        advanceTimer -= Time.deltaTime;

        if(advanceTimer > 0f)
            return;

        if(isStepMoving)
            BeginPausePhase();
        else
            BeginStepPhase();
    }

    private void BeginPausePhase()
    {
        isStepMoving = false;
        advanceTimer = pauseDuration;

        if(movement != null)
            movement.Stop();

        if(animationController != null)
            animationController.SetMoving(false);
    }

    private void BeginStepPhase()
    {
        isStepMoving = true;
        advanceTimer = stepDuration;

        if(movement != null)
        {
            movement.SetMovementEnabled(true);
            movement.SetMoveDirection(advanceDirection);
        }

        if(facingController != null)
            facingController.SetFacing(advanceDirection);

        if(animationController != null)
            animationController.SetMoving(true);
    }

    private bool TryBeginAttack()
    {
        if(targetDetector == null)
            return false;

        if(combat == null)
            return false;

        if(!combat.CanAttack)
            return false;

        if(!targetDetector.CanAttackTarget(advanceDirection))
            return false;

        BeginAttack();

        return true;
    }
}
