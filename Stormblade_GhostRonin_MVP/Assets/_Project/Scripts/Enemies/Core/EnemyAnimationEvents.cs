using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private EnemyCombat combat;

    private void Awake()
    {
        if(combat == null)
            combat = GetComponentInParent<EnemyCombat>();
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
}
