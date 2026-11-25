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
    [SerializeField] private EnemyHitReactionController hitReactionController;
    [SerializeField] private Health health;

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
    [SerializeField] private bool resumeAdvanceAfterHitReaction;

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

        if(hitReactionController == null)
            hitReactionController = GetComponent<EnemyHitReactionController>();

        if(health == null)
            health = GetComponent<Health>();

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

        // Integração com a reação de Hit
        if(hitReactionController != null)
        {
            hitReactionController.OnHitReactionStarted += HandleHitReactionStarted;

            hitReactionController.OnHitReactionFinished += HandleHitReactionFinished;
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

        if(hitReactionController != null)
        {
            hitReactionController.OnHitReactionStarted -= HandleHitReactionStarted;

            hitReactionController.OnHitReactionFinished -= HandleHitReactionFinished;
        }
    }

    private void Update()
    {
        if(health != null && health.IsDead)
            return;

        //Bloqueia decisões da IA durante o Hit
        if(hitReactionController != null && hitReactionController.IsHitReacting)
            return;

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
        if(health != null && health.IsDead)
            return;

        // Se estiver reagindo a Hit, adia a retomada
        if(hitReactionController != null && hitReactionController.IsHitReacting)
        {
            resumeAdvanceAfterHitReaction = true;
            return;
        }

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
        if(health != null && health.IsDead)
            return;

        if(hitReactionController != null && hitReactionController.IsHitReacting)
            return;

        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        StopAdvance();

        if(!combat.TryStartAttack())
            BeginAdvance();
    }

    private void HandleAttackFinished()
    {
        if(!isActiveAndEnabled)
            return;

        if(health != null && health.IsDead)
            return;

        resumeAdvanceAfterHitReaction = false;

        BeginAdvance();
    }

    private void HandleHitReactionStarted()
    {
        if(!isActiveAndEnabled)
            return;

        if(health != null && health.IsDead)
            return;

        // Registra se o inimigo deverá retomar o avanço. Considera também um ataque que já estava ativo ou uma retomada pendente após troca de lane
        resumeAdvanceAfterHitReaction = resumeAdvanceAfterHitReaction || isAdvancing || (combat != null && (combat.IsAttacking || combat.WasAttackInterruptedByHit)) || resumeAdvanceAfterLaneTransition;

        // Interrompe o ciclo de pequenos passos
        StopAdvance();

        Debug.Log($"[ONI BRAIN] {name}: avanço interrompido por Hit",
        this);
    }

    private void HandleHitReactionFinished()
    {
        if(!isActiveAndEnabled)
            return;

        if(health != null && health.IsDead)
            return;

        if(hitReactionController != null && hitReactionController.IsHitReacting)
            return;

        if(!resumeAdvanceAfterHitReaction)
            return;

        // Enquanto o combate considerar um ataque ativo, aguardaremos seu encerramento
        if(combat != null && combat.IsAttacking)
            return;

        resumeAdvanceAfterHitReaction = false;

        BeginAdvance();

        Debug.Log($"[ONI BRAIN] {name}: reação concluída.", 
        this);
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
