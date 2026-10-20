using UnityEngine;

public class PlayerLaneTargetDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundCheck;

    [Header("Lane Masks")]
    [SerializeField] private LayerMask frontLineMask;
    [SerializeField] private LayerMask backLineMask;

    [Header("Search Settings")]
    [SerializeField] private float searchUpDistance = 3f;
    [SerializeField] private float searchDownDistance = 6f;
    [SerializeField] private float maxVerticalDistance = 4f;

    [Header("Runtime Debug")]
    [SerializeField] private bool lastSearchFoundTarget;
    [SerializeField] private Vector2 lastTargetPosition;

    public bool TryFindTarget(GameplayLane targetLane, out Vector2 targetPlayerPosition)
    {
        targetPlayerPosition = default;

        if(groundCheck == null)
        {
            Debug.LogWarning("PlayerLaneTargetDetector: GroundCheck não configurado.", this);

            return false;
        }

        LayerMask targetMask = targetLane == GameplayLane.Front ? frontLineMask : backLineMask;

        Vector2 searchOrigin = (Vector2)groundCheck.position + Vector2.up * searchUpDistance;

        float totalSearchDistance = searchUpDistance + searchDownDistance;

        RaycastHit2D hit = Physics2D.Raycast(searchOrigin, Vector2.down, totalSearchDistance, targetMask);

        if(hit.collider == null)
        {
            RegisterFailedSearch();

            return false;
        }

        float verticalDistance = Mathf.Abs(hit.point.y - groundCheck.position.y);

        if(verticalDistance > maxVerticalDistance)
        {
            RegisterFailedSearch();

            return false;
        }

        float playerToGroundCheckOffset = transform.position.y - groundCheck.position.y;

        targetPlayerPosition = new Vector2(transform.position.x, hit.point.y + playerToGroundCheckOffset);

        lastSearchFoundTarget = true;
        lastTargetPosition = targetPlayerPosition;

        return true;
    }

    private void RegisterFailedSearch()
    {
        lastSearchFoundTarget = false;
        lastTargetPosition = Vector2.zero;
    }

    [ContextMenu("TEST/Detect Front")]
    private void TestDetectFront()
    {
        TestDetection(GameplayLane.Front);
    }

    [ContextMenu("TEST/Detect Back")]
    private void TestDetectionBack()
    {
        TestDetection(GameplayLane.Back);
    }

    private void TestDetection(GameplayLane lane)
    {
        if(TryFindTarget(lane, out Vector2 target))
        {
            Debug.Log($"[LANE TARGET] {lane} encontrado em {target}.", this);

            return;
        }

        Debug.Log($"[LANE TARGET] Nenhum terreno válido encontrado em {lane}.", this);
    }

    private void OnDrawGizmosSelected()
    {
        if(groundCheck == null)
            return;

        Vector3 origin = groundCheck.position + Vector3.up * searchUpDistance;

        Vector3 end = groundCheck.position - Vector3.up * searchDownDistance;

        Gizmos.DrawLine(origin, end);

        if(lastSearchFoundTarget)
            Gizmos.DrawWireSphere(lastTargetPosition, 0.15f);
    }


}
