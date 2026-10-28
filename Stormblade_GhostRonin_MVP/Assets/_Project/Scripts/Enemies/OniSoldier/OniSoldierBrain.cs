using UnityEngine;

public class OniSoldierBrain : MonoBehaviour
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

        if(facingController != null)
        {
            advanceDirection = advanceDirection >= 0f ? 1f : -1f;
            
            facingController.SetFacing(advanceDirection);
        }
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

        advanceDirection = advanceDirection >= 0f ? 1f : -1f;

        isAdvancing = true;

        movement.SetMovementEnabled(true);
        movement.SetMoveDirection(advanceDirection);

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
}
