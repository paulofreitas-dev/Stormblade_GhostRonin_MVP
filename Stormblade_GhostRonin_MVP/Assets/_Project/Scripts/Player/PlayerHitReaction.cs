using System.Text;
using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Pushback Settings")]
    [SerializeField] private float pushbackSpeed = 3.5f;
    [SerializeField] private float pushbackDuration = 0.12f;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();
        
        if(playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if(health != null)
            health.OnDamaged += HandleDamageReaction;
    }

    private void OnDisable()
    {
        if(health != null)
            health.OnDamaged -= HandleDamageReaction;
    }

    private void HandleDamageReaction(DamageData damageData)
    {
        if(health == null || health.IsDead)
            return;

        if(playerMovement == null)
            return;

        float pushDirection = CalculatePushDirection(damageData.sourceTransform);

        playerMovement.StartDamagePushback(pushDirection, pushbackSpeed, pushbackDuration);
    }

    private float CalculatePushDirection(Transform damageSource)
    {
        if(damageSource != null)
        {
            float horizontalDifference = transform.position.x - damageSource.position.x;

            if(Mathf.Abs(horizontalDifference) > 0.01f)
                return Mathf.Sign(horizontalDifference);
        }

        // DecoderFallback para fontes sem posição válida ou quando fonte e player estiverem praticamente no mesmo eixo X.
        return playerMovement.IsFacingRight ? -1f : 1f;
    }
}
