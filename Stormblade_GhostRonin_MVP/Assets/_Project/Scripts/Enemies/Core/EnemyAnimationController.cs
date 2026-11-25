using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Health health;
    [SerializeField] private EnemyHitReactionController hitReactionController;
    [SerializeField] private EnemyLaneMovement laneMovement;
    [SerializeField] private EnemyLaneController laneController;

    private static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    private static readonly int BasicAttackHash = Animator.StringToHash("attackBasic");
    private static readonly int HitHash = Animator.StringToHash("hit");
    private static readonly int DeathHash = Animator.StringToHash("death");
    private static readonly int IsChangingLaneHash = Animator.StringToHash("isChangingLane");
    private static readonly int LaneToBackHash = Animator.StringToHash("laneToBack");

    private void Awake()
    {
        if(animator == null)
            animator = GetComponentInChildren<Animator>();

        if(health == null)
            health = GetComponent<Health>();

        if(hitReactionController == null)
            hitReactionController = GetComponent<EnemyHitReactionController>();

        if(laneMovement == null)
            laneMovement = GetComponent<EnemyLaneMovement>();

        if(laneController == null)
            laneController = GetComponent<EnemyLaneController>();
    }

    private void OnEnable()
    {
        if(hitReactionController != null)
            hitReactionController.OnHitReactionStarted += HandleHitReactionStarted;

        if(laneMovement != null)
        {
            laneMovement.OnLaneMovementStarted += HandleLaneMovementStarted;
            laneMovement.OnLaneMovementFinished += HandleLaneMovementEnded;
            laneMovement.OnLaneMovementCancelled += HandleLaneMovementEnded;
        }
    }

    private void OnDisable()
    {
        if(hitReactionController != null)
            hitReactionController.OnHitReactionFinished += HandleHitReactionStarted;

        if(laneMovement != null)
        {
            laneMovement.OnLaneMovementStarted -= HandleLaneMovementStarted;
            laneMovement.OnLaneMovementFinished -= HandleLaneMovementEnded;
            laneMovement.OnLaneMovementCancelled -= HandleLaneMovementEnded;
        }
    }

    public void SetMoving(bool isMoving)
    {
        if(animator == null)
            return;

        if(isMoving && health != null && health.IsDead)
            return;

        if(isMoving && hitReactionController != null && hitReactionController.IsHitReacting)
            return;

        animator.SetBool(IsMovingHash, isMoving);
    }

    public void PlayBasicAttack()
    {
        if(animator == null)
            return;

        if(health != null && health.IsDead)
            return;

        if(hitReactionController != null && hitReactionController.IsHitReacting)
            return;

        animator.SetTrigger(BasicAttackHash);
    }

    private void HandleHitReactionStarted()
    {
        PlayHit();
    }

    private void PlayHit()
    {
        if(animator == null)
            return;

        if(health == null || health.IsDead)
            return;

        animator.ResetTrigger(BasicAttackHash);
        animator.SetBool(IsMovingHash, false);

        animator.SetTrigger(HitHash);
    }

    public void PlayDeath()
    {
        if(animator == null)
            return;

        // Death tem prioridade sobre Hit e Attack
        animator.ResetTrigger(HitHash);
        animator.ResetTrigger(BasicAttackHash);
        animator.SetBool(IsMovingHash, false);

        animator.SetTrigger(DeathHash);
    }

    private void HandleLaneMovementStarted()
    {
        if(animator == null || laneController == null)
            return;

        // Interrompe visualmente o movimento convencional
        animator.SetBool(IsMovingHash, false);

        // Define a pose conforme o destino da transição
        bool goingToBack = laneController.TargetLane == GameplayLane.Back;

        animator.SetBool(LaneToBackHash, goingToBack);
        animator.SetBool(IsChangingLaneHash, true);
    }

    private void HandleLaneMovementEnded()
    {
        if(animator == null)
            return;

        // Preserva uma nova transição que já tenha começado
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        // Libera o Animator para retornar ao Idle
        animator.SetBool(IsChangingLaneHash, false);
    }
}
