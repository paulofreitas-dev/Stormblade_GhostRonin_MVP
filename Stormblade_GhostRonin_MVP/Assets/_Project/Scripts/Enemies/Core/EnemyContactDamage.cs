using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D contactCollider;
    [SerializeField] private EnemyLaneController enemyLaneController;
    [SerializeField] private Transform damageSource;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    [Header("Runtime Debug")]
    [SerializeField] private bool damageEnabled = true;

    public bool DamageEnabled => damageEnabled;

    private void Awake()
    {
        if(enemyLaneController == null)
            enemyLaneController = GetComponentInParent<EnemyLaneController>();

        if(contactCollider == null)
            contactCollider = GetComponent<Collider2D>();

        if(damageSource == null)
        {
            Health ownerHealth = GetComponentInParent<Health>();

            if(ownerHealth != null)
                damageSource = ownerHealth.transform;
        }

        if(contactCollider != null)
            contactCollider.isTrigger = true;
    }

    public void SetDamageEnabled(bool enabled)
    {
        damageEnabled = enabled;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(!damageEnabled)
            return;

        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if(hurtbox == null)
            return;

        PlayerLaneController playerLaneController = hurtbox.GetComponentInParent<PlayerLaneController>();

        if(playerLaneController == null)
            return;

        if(playerLaneController.IsTransitioning)
            return;

        if(!IsPlayerInSameLane(playerLaneController.CurrentLane))
            return;

        DamageData damageData = new DamageData(damageAmount, damageSource);

        hurtbox.ReceiveHit(damageData);

        Debug.Log($"[ONI CONTACT] {name} causou {damageAmount} de dano.", this);
    }

    private bool IsPlayerInSameLane(GameplayLane playerLane)
    {
        if(enemyLaneController == null)
            return false;
        
        return enemyLaneController.CurrentLane == playerLane;
    }
}
