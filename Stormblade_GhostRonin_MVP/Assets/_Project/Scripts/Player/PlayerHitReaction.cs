using System.Text;
using UnityEngine;

public class PlayerHitReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerRespawnController respawnController;

    [Header("Pushback Settings")]
    [SerializeField] private float pushbackSpeed = 3.5f;
    [SerializeField] private float pushbackDuration = 0.12f;

    [SerializeField] private bool isHitReacting;

    public bool IsHitReacting => isHitReacting;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();
        
        if(playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if(inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();

        if(respawnController == null)
            respawnController = GetComponent<PlayerRespawnController>();
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

        BeginHitReaction();

        float pushDirection = CalculatePushDirection(damageData.sourceTransform);

        playerMovement.StartDamagePushback(pushDirection, pushbackSpeed, pushbackDuration);
    }

    public void BeginHitReaction()
    {
        isHitReacting = true;

        if(inputReader != null)
            inputReader.SetGamePlayInputBlocked(true);

        Debug.Log("[HIT LOCK] Ativado.");
    }

    public void FinishHitReaction()
    {
        if(!isHitReacting)
            return;
        
        isHitReacting = false;

        if(health != null && health.IsDead)
        {
            Debug.Log("[HIT LOCK] Hit terminou durante morte." + "Input permanece bloqueado.");

            return;
        }

        if(respawnController != null && respawnController.IsRespawning)
        {
            Debug.Log("[HIT LOCK] Hit terminou durante Respawn." + "Input permanece bloqueado.");

            return;
        }

        if(inputReader != null)
            inputReader.SetGamePlayInputBlocked(false);

        Debug.Log("[HIT LOCK] Desativado.");
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
