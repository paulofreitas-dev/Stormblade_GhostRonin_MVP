using UnityEngine;

public class EnemyDeathController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private OniSoldierBrain brain;
    [SerializeField] private EnemyDespawnController despawnController;
    [SerializeField] private Collider2D hurtboxCollider;
    [SerializeField] private EnemyRewardDrop rewardDrop;

    [Header("Runtime Debug")]
    [SerializeField] private bool isDying;

    public bool IsDying => isDying;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();

        if(movement == null)
            movement = GetComponent<EnemyMovement>();

        if(combat == null)
            combat = GetComponent<EnemyCombat>();

        if(animationController == null)
            animationController = GetComponent<EnemyAnimationController>();

        if(brain == null)
            brain = GetComponent<OniSoldierBrain>();

        if(despawnController == null)
            despawnController = GetComponent<EnemyDespawnController>();

        if(rewardDrop == null)
            rewardDrop = GetComponent<EnemyRewardDrop>();
    }

    private void OnEnable()
    {
        if(health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if(health != null)
            health.OnDied -= HandleDeath;
    }

    private void HandleDeath()
    {
        if(isDying)
            return;
        
        isDying = true;

        if(brain != null)
            brain.enabled = false;

        if(movement != null)
        {
            movement.SetMovementEnabled(false);
            movement.Stop();
        }

        if(combat != null)
            combat.DisableAllDamage();

        if(despawnController != null)
            despawnController.enabled = false;

        if(hurtboxCollider != null)
            hurtboxCollider.enabled = false;

        if(animationController != null)
            animationController.PlayDeath();

        Debug.Log($"[ENEMY DEATH] {name} iniciou morte.", this);
    }

    public void AnimationEvent_DeathFinished()
    {
        if(!isDying)
            return;

        if(rewardDrop != null)
            rewardDrop.SpawnReward();

        Debug.Log($"[ENEMY DEATH] {name} finalizou morte.", this);

        Destroy(gameObject);
    }
}
