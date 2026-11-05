using UnityEngine;

public class OniSoldierBrain : MonoBehaviour, IEnemySpawnDirectionConfigurable
{
    [Header("References")]
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private EnemyTargetDetector targetDetector;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyFacingController facingController;

    [Header("Advance Settings")]
    [SerializeField] private float advanceDirection = -1f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isAdvancing;

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

        // ApplyAdvanceDirection();
    }

    private void Start()
    {
        if(combat != null)
            combat.OnAttackFinished += HandleAttackFinished;

        BeginAdvance();
    }

    private void OnDestroy()
    {
        if(combat != null)
            combat.OnAttackFinished -= HandleAttackFinished;
    }

    private void Update()
    {
        if(!isAdvancing)
            return;

        if(targetDetector == null || combat == null)
            return;

        if(!combat.CanAttack)
            return;

        if(!targetDetector.CanAttackTarget(advanceDirection))
            return;

        BeginAttack();
    }

    public void BeginAdvance()
    {
        if(movement == null)
            return;

        isAdvancing = true;

        movement.SetMovementEnabled(true);
        
        ApplyAdvanceDirection();

        if(animationController != null)
            animationController.SetMoving(true);
    }

    public void StopAdvance()
    {
        isAdvancing = false;

        if(movement != null)
            movement.Stop();

        if(animationController != null)
            animationController.SetMoving(false);
    }

    private void BeginAttack()
    {
        StopAdvance();

        if(!combat.TryStartAttack())
            BeginAdvance();
    }

    private void HandleAttackFinished()
    {
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
}
