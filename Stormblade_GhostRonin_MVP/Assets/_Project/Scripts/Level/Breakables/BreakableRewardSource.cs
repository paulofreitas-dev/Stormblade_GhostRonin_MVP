using UnityEngine;

public class BreakableRewardSource : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Health health;

    [Header("Breakable Parts")]
    [SerializeField] private Collider2D physicalCollider;
    [SerializeField] private GameObject hurtboxObject;
    [SerializeField] private GameObject visualObject;

    [Header("Reward")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string destroyTriggerName = "Destroy";

    private bool isBroken;

    private bool destructionCompleted;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if(animator == null && visualObject != null)
            animator = visualObject.GetComponent<Animator>();

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
        PlayDestructionAnimation();

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

    private void SpawnReward()
    {
        if(rewardPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name}: nenhum prefab de recompensa foi configurado.");

            return;
        }

        Vector3 spawnPosition = transform.position;

        if(rewardSpawnPoint != null)
            spawnPosition = rewardSpawnPoint.position;

        Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);
    }

    private void PlayDestructionAnimation()
    {
        if(animator == null)
        {
            Debug.LogWarning($"{gameObject.name}: Animator não configurado." + "Concluindo destruição imediatamente.");

            CompleteDestruction();
        }

        animator.SetTrigger(destroyTriggerName);
    }

    public void CompleteDestruction()
    {
        if(destructionCompleted)
            return;

        destructionCompleted = true;
        
        DisableVisual();
        SpawnReward();

        Debug.Log($"{gameObject.name}: destruição concluída.");
    }
}
