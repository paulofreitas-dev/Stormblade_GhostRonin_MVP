using Unity.VisualScripting;
using UnityEngine;

public class EnemyDespawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private Health health;

    [Header("Despawn Settings")]
    [SerializeField, Range(0f, 0.5f)] private float horizontalViewportMargin = 0.10f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isDespawning;
    [SerializeField] private float viewportX;

    public bool IsDespawning => isDespawning;

    private void Awake()
    {
        if(targetCamera == null)
            targetCamera = Camera.main;

        if(movement == null)
            movement = GetComponent<EnemyMovement>();

        if(combat == null)
            combat = GetComponent<EnemyCombat>();
        
        if(health == null)
            health = GetComponent<Health>();
    }

    private void Update()
    {
        if(isDespawning)
            return;

        if(targetCamera == null || movement == null)
            return;

        if(health != null && health.IsDead)
            return;

        float travelDirection = movement.LastNonZeroDirection;

        if(Mathf.Abs(travelDirection) < 0.01f)
            return;

        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(transform.position);

        if(viewportPosition.z < 0f)
            return;

        viewportX = viewportPosition.x;

        if(HasExitedPlayArea(viewportX, travelDirection))
        {
            Despawn();
        }
    }

    private bool HasExitedPlayArea(float currentViewportX, float travelDirection)
    {
        if(travelDirection < 0f)
            return currentViewportX < -horizontalViewportMargin;

        if(travelDirection > 0f)
            return currentViewportX > 1f + horizontalViewportMargin;

        return false;
    }

    private void Despawn()
    {
        if(isDespawning)
            return;

        isDespawning = true;

        if(movement != null)
            movement.SetMovementEnabled(false);

        if(combat != null)
            combat.DisableAllDamage();

        Debug.Log($"[ENEMY DESPAWN] {name} saiu da área.", this);

        Destroy(gameObject);
    }
}
