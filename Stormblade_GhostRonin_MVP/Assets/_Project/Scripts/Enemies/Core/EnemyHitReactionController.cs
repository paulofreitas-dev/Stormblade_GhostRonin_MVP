using UnityEngine;
using System;

public class EnemyHitReactionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyFacingController facingController;
    [SerializeField] private EnemyLaneMovement laneMovement;

    [Header("Pushback Settings")]
    [SerializeField] private float pushbackSpeed = 3.5f;
    [SerializeField] private float pushbackDuration = 0.12f;

    [Header("Runtime Debug")]
    [SerializeField] private bool isHitReacting;

    public bool IsHitReacting => isHitReacting;

    public event Action OnHitReactionStarted;
    public event Action OnHitReactionFinished;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();

        if(enemyMovement == null)
            enemyMovement = GetComponent<EnemyMovement>();

        if(facingController == null)
            facingController = GetComponent<EnemyFacingController>();

        if(laneMovement == null)
            laneMovement = GetComponent<EnemyLaneMovement>();
    }

    private void OnEnable()
    {
        if(health == null)
        {
            Debug.Log("[ENEMY HIT] Health não configurado.",
            this);

            return;
        }

        health.OnDamaged += HandleDamageReaction;
        health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if(health == null)
            return;

        health.OnDamaged -= HandleDamageReaction;
        health.OnDied -= HandleDeath;
    }

    private void HandleDamageReaction(DamageData damageData)
    {
        // Dano fatal não inicia reação de Hit
        if(health == null || health.IsDead)
            return;

        BeginHitReaction();

        // Orienta o Oni para a origem do dano
        if(facingController != null && damageData.sourceTransform != null)
        {
            float directionToAttacker = damageData.sourceTransform.position.x - transform.position.x;

            if(Mathf.Abs(directionToAttacker) > 0.01f)
                facingController.SetFacing(directionToAttacker);
        }

        if(enemyMovement == null)
            return;

        // Evita conflito entre pushback e deslocamento físico de troca de lane
        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
            return;

        float pushDirection = CalculatePushDirection(damageData.sourceTransform);

        enemyMovement.StartDamagePushback(pushDirection, pushbackSpeed, pushbackDuration);
    }

    public void BeginHitReaction()
    {
        if(health == null || health.IsDead)
            return;

        // Evita iniciar a mesma reação duas vezes
        if(isHitReacting)
            return;

        isHitReacting = true;

        OnHitReactionStarted?.Invoke();

        Debug.Log($"[ENEMY HIT] {name}: reação iniciada.",
        this);
    }

    public void FinishHitReaction()
    {
        if(!isHitReacting)
            return;

        isHitReacting = false;

        // Nunca libera o comportamento de um inimigo morto
        if(health == null || health.IsDead)
            return;

        OnHitReactionFinished?.Invoke();

        Debug.Log($"[ENEMY HIT] {name}: reação concluída.",
        this);
    }

    private float CalculatePushDirection(Transform damageSource)
    {
        if(damageSource != null)
        {
            float horizontalDifference = transform.position.x - damageSource.position.x;

            if(Mathf.Abs(horizontalDifference) > 0.01f)
                return Mathf.Sign(horizontalDifference);
        }

        // Fallback: recua na direção contrária aquela para a qual o inimigo está olhando
        if(facingController != null)
            return facingController.CurrentDirection > 0 ? -1f : 1f;

        // Sem origem ou direção conhecida, não aplica recuo arbitrário
        return 0f;
    }

    private void HandleDeath()
    {
        // A morte interrompe o estado de Hit, sem disparar o evento de conclusão
        isHitReacting = false;
    }

#if UNITY_EDITOR
    [ContextMenu("TEST/Finish Hit Reaction")]
    private void TestFinishHitReaction()
    {
        if(!Application.isPlaying)
            return;

        FinishHitReaction();
    }
#endif
}
