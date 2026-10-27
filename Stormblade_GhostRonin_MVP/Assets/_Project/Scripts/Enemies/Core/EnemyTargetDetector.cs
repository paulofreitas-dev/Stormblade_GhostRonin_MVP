using UnityEngine;

public class EnemyTargetDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLaneController playerLaneController;
    [SerializeField] private Collider2D enemyBodyCollider;

    [Header("Attack Detection")]
    [SerializeField] private float attackRange = 1.5f;

    [Header("Runtime Debug")]
    [SerializeField] private float horizontalDistance;
    [SerializeField] private bool sameLane;
    [SerializeField] private bool targetAhead;
    [SerializeField] private bool targetInRange;

    private Transform target;

    private int frontLaneBodyLayer;
    private int backLaneBodyLayer;

    public bool TargetInRange => targetInRange;

    private void Awake()
    {
        frontLaneBodyLayer = LayerMask.NameToLayer("FrontLaneBody");
        backLaneBodyLayer = LayerMask.NameToLayer("BackLaneBody");

        if(playerLaneController == null)
            playerLaneController = FindFirstObjectByType<PlayerLaneController>();

        if(playerLaneController != null)
            target = playerLaneController.transform;

        if(enemyBodyCollider == null)
            Debug.LogWarning($"{name}: Enemy BodyCollider não configurado.", this);
    }

    public bool CanAttackTarget(float advanceDirection)
    {
        ResetRuntimeDebug();

        if(target == null || playerLaneController == null || enemyBodyCollider == null)
            return false;

        //Durante a troca de lane, o Player não é considerado
        //um alvo válido para este ataque lane.
        if(playerLaneController.IsTransitioning)
            return false;

        if(!TryGetEnemyLane(out GameplayLane enemyLane))
            return false;

        sameLane = enemyLane == playerLaneController.CurrentLane;

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

    private bool TryGetEnemyLane(out GameplayLane lane)
    {
        lane = GameplayLane.Front;

        int bodyLayer = enemyBodyCollider.gameObject.layer;

        if(bodyLayer == frontLaneBodyLayer)
        {
            lane = GameplayLane.Front;
            return true;
        }

        if(bodyLayer == backLaneBodyLayer)
        {
            lane = GameplayLane.Back;
            return true;
        }

        return false;
    }

    private void ResetRuntimeDebug()
    {
        horizontalDistance = 0f;
        sameLane = false;
        targetAhead = false;
        targetInRange = false;
    }
}
