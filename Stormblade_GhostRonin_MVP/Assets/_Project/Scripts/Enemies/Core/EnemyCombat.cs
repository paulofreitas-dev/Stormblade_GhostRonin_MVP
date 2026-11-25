using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hitbox attackHitbox;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private EnemyContactDamage contactDamage;
    [SerializeField] private EnemyHitReactionController hitReactionController;
    [SerializeField] private Health health;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.8f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isAttacking;
    [SerializeField] private float nextAttackAllowedTime;
    [SerializeField] private bool isHitLocked;
    [SerializeField] private bool wasAttackInterruptedByHit;

    public bool IsAttacking => isAttacking;
    public bool IsHitLocked => isHitLocked;
    public bool WasAttackInterruptedByHit => wasAttackInterruptedByHit;

    public bool CanAttack => !isAttacking && !isHitLocked && (health == null || !health.IsDead) && Time.time >= nextAttackAllowedTime;

    public event Action OnAttackFinished;

    private void Awake()
    {
        if(animationController == null)
            animationController = GetComponent<EnemyAnimationController>();

        if(contactDamage == null)
            contactDamage = GetComponentInChildren<EnemyContactDamage>(true);

        if(hitReactionController == null)
            hitReactionController = GetComponent<EnemyHitReactionController>();

        if(health == null)
            health = GetComponent<Health>();

        if(attackHitbox != null)
            attackHitbox.DisableHitbox();
        

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(true);
    }

    private void OnEnable()
    {
        if(hitReactionController != null)
        {
            hitReactionController.OnHitReactionStarted += HandleHitReactionStarted;

            hitReactionController.OnHitReactionFinished += HandleHitReactionFinished;
        }
    }

    private void OnDisable()
    {
        if(hitReactionController != null)
        {
            hitReactionController.OnHitReactionStarted -= HandleHitReactionStarted;

            hitReactionController.OnHitReactionFinished -= HandleHitReactionFinished;
        }
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

    private void HandleHitReactionStarted()
    {
        InterruptAttackForHit();
    }

    private void HandleHitReactionFinished()
    {
        // A morte nunca deve restaurar o dano
        if(health != null && health.IsDead)
            return;

        if(!isHitLocked)
            return;

        isHitLocked = false;
        wasAttackInterruptedByHit = false;

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(true);
    }

    public void InterruptAttackForHit()
    {
        if(health != null && health.IsDead)
            return;

        // Guarda se havia um ataque ativo antes de interromper seu estado
        wasAttackInterruptedByHit = isAttacking;

        isHitLocked = true;

        if(attackHitbox != null)
            attackHitbox.DisableHitbox();

        isAttacking = false;

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(false);

        // Preserva um pequeno intervalo antes de permitir o próximo ataque
        nextAttackAllowedTime = Mathf.Max(nextAttackAllowedTime, Time.time + attackCooldown);
    }

    public void DisableAllDamage()
    {
        if(attackHitbox != null)
            attackHitbox.DisableHitbox();

        if(contactDamage != null)
            contactDamage.SetDamageEnabled(false);

        isAttacking = false;
        isHitLocked = true;
        wasAttackInterruptedByHit = false;
    }
}
