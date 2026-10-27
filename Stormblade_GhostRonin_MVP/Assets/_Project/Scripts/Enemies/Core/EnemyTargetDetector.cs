using UnityEngine;

public class EnemyTargetDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLaneController playerLaneController;
    [SerializeField] private EnemyLaneController enemyLaneController;

    [Header("Attack Detection")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("Runtime Debug")]
    [SerializeField] private float horizontalDistance;
    [SerializeField] private bool sameLane;
    [SerializeField] private bool targetAhead;
    [SerializeField] private bool targetInRange;

    private Transform target;

    public bool TargetInRange => targetInRange;

    private void Awake()
    {
        
        if(playerLaneController == null)
            playerLaneController = FindFirstObjectByType<PlayerLaneController>();

        if(playerLaneController != null)
            target = playerLaneController.transform;

        if(enemyLaneController == null)
            enemyLaneController = GetComponent<EnemyLaneController>();
    }

    public bool CanAttackTarget(float advanceDirection)
    {
        ResetRuntimeDebug();

        if(target == null || playerLaneController == null || enemyLaneController == null)
            return false;

        //Durante a troca de lane, o Player não é considerado
        //um alvo válido para este ataque lane.
        if(playerLaneController.IsTransitioning)
            return false;

        sameLane = enemyLaneController.CurrentLane == playerLaneController.CurrentLane;

        if(!sameLane)
            return false;

        float differenceX = target.position.x - transform.position.x;

        horizontalDistance = Mathf.Abs(differenceX);

        targetAhead = differenceX * advanceDirection > 0f;

        if(!targetAhead)
            return false;

        targetInRange = horizontalDistance <= attackRange;

        return targetInRange;
    }

    private void ResetRuntimeDebug()
    {
        horizontalDistance = 0f;
        sameLane = false;
        targetAhead = false;
        targetInRange = false;
    }
}
