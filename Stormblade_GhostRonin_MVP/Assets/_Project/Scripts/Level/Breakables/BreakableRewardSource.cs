using UnityEngine;

public class BreakableRewardSource : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Health health;

    [Header("Breakable Parts")]
    [SerializeField] private Collider2D physicalCollider;
    [SerializeField] private GameObject hurtboxObject;
    [SerializeField] private GameObject visualObject;

    private bool isBroken;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (health == null)
            Debug.LogError($"{gameObject.name}: BreakableRewardSource não encontrou Health.");
    }
        
    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        if (isBroken)
            return;

        isBroken = true;

        DisableDamageReception();
        DisablePhysicalCollision();
        DisableVisual();

        Debug.Log($"{gameObject.name}: fonte quebrável destrúida.");
    }

    private void DisableDamageReception()
    {
        if (hurtboxObject != null)
            hurtboxObject.SetActive(false);
    }

    private void DisablePhysicalCollision()
    {
        if (physicalCollider != null)
            physicalCollider.enabled = false;
    }

    private void DisableVisual()
    {
        if (visualObject != null)
            visualObject.SetActive(false);
    }
}
