using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hitbox attackHitbox;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private EnemyContactDamage contactDamage;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.8f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isAttacking;
    [SerializeField] private float nextAttackAllowedTime;

    public bool IsAttacking => isAttacking;

    public bool CanAttack => !isAttacking && Time.time >= nextAttackAllowedTime;

    public event Action OnAttackFinished;

    private void Awake()
    {
        if(animationController == null)
            animationController = GetComponent<EnemyAnimationController>();

        if(attackHitbox != null)
            attackHitbox.DisableHitbox();
        
        if(contactDamage == null)
            contactDamage = GetComponentInChildren<EnemyContactDamage>(true);

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(true);
    }

    public bool TryStartAttack()
    {
        if(!CanAttack)
            return false;

        isAttacking = true;

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(false);

        if(attackHitbox != null)
            attackHitbox.DisableHitbox();

        if(animationController != null)
            animationController.PlayBasicAttack();

        return true;
    }

    public void AnimationEvent_EnableAttackHitbox()
    {
        if(!isAttacking)
            return;

        if(attackHitbox != null)
            attackHitbox.EnableHitbox();
    }

    public void AnimationEvent_DisableAttackHitbox()
    {
        if(attackHitbox != null)
            attackHitbox.DisableHitbox();
    }

    public void AnimationEvent_EndAttack()
    {
        if(!isAttacking)
            return;

        if(attackHitbox != null)
            attackHitbox.DisableHitbox();

        isAttacking = false;

        nextAttackAllowedTime = Time.time + attackCooldown;

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(true);

        OnAttackFinished?.Invoke();
    }

    public void DisableAllDamage()
    {
        if(attackHitbox != null)
            attackHitbox.DisableHitbox();

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(false);

        isAttacking = false;
    }
}
