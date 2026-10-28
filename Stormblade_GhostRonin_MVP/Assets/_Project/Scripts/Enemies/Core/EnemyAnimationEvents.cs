using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyCombat combat;
    [SerializeField] private EnemyDeathController deathController;

    private void Awake()
    {
        if(combat == null)
            combat = GetComponentInParent<EnemyCombat>();

        if(deathController == null)
            deathController = GetComponent<EnemyDeathController>();
    }

    public void EnableAttackHitbox()
    {
        if(combat != null)
            combat.AnimationEvent_EnableAttackHitbox();
    }

    public void DisableAttackHitbox()
    {
        if(combat != null)
            combat.AnimationEvent_DisableAttackHitbox();
    }

    public void EndAttack()
    {
        if(combat != null)
            combat.AnimationEvent_EndAttack();
    }

    public void AnimationEvent_DeathFinished()
    {
        if(deathController != null)
            deathController.AnimationEvent_DeathFinished();
    }
}
