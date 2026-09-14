using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerLifePoints lifePoints;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CameraTargetController cameraTargetController;

    [Header("Respawn Points")]
    [SerializeField] private Transform initialRespawnPoint;
    [SerializeField] private Transform currentRespawnPoint;

    [Header("Respawn State")]
    [SerializeField] private bool respawnPending;
    [SerializeField] private bool isRespawning;

    public Transform CurrentRespawnPoint => currentRespawnPoint;
    public bool RespawnPending => respawnPending;
    public bool IsRespawning => isRespawning;

    private void Awake()
    {
        if(inputReader == null)
            inputReader = GetComponent<PlayerInputReader>();

        if(animationController == null)
            animationController = GetComponentInChildren<PlayerAnimationController>();

        if(health == null)
            health = GetComponent<Health>();

        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        if(lifePoints == null)
            lifePoints = GetComponent<PlayerLifePoints>();

        if(initialRespawnPoint == null)
        {
            Debug.LogWarning("PlayerRespawnController: Initial Respawn Point não configurado.");

            return;
        }

        currentRespawnPoint = initialRespawnPoint;
        respawnPending = false;
        isRespawning = false;
    }

    private void OnEnable()
    {
        if(health != null)
            health.OnDied += HandlePlayerDeath;

        if(animationController != null)
            animationController.OnDeathAnimationFinished += HandleDeathAnimationFinished;

    }

    private void OnDisable()
    {
        if(health != null)
            health.OnDied -= HandlePlayerDeath;

        if(animationController != null)
            animationController.OnDeathAnimationFinished -= HandleDeathAnimationFinished;
    }

    private void HandleDeathAnimationFinished()
    {
        if(!respawnPending)
            return;

        if(lifePoints == null)
        {
            respawnPending = false;

            Debug.Log("PlayerRespawnController: PlayerLifePoints não encontrado.");

            return;
        }

        if (lifePoints.IsGameOver)
        {
            respawnPending = false;

            Debug.Log("PlayerRespawnController: Death terminou, " + "mas é Game Over. Respawn cancelado.");

            return;
        }

        Debug.Log("PlayerRespawnController: Death concluída. " + "Iniciando Respawn.");

        MoveToCurrentRespawnPoint();

    }

    public void SetCheckpoint(Transform newRespawnPoint)
    {
        if(newRespawnPoint == null)
            return;

        currentRespawnPoint = newRespawnPoint;

        Debug.Log($"Checkpoint atualizado: {newRespawnPoint.name} | " + $"RespawnPoint: {newRespawnPoint.name}");
    }

    private void HandlePlayerDeath()
    {
        if(respawnPending)
            return;

        respawnPending = true;

        if(currentRespawnPoint == null)
        {
            respawnPending = false;
            
            Debug.LogWarning("PlayerRespawnController: morte detectada, mas não existe Respawn Point.");

            return;
        }
    }

    private void MoveToCurrentRespawnPoint()
    {
        if(currentRespawnPoint == null)
            return;

        if(rb == null)
            return;

        BeginRespawnLock();

        rb.position = currentRespawnPoint.position;

        if(cameraTargetController != null)
            cameraTargetController.ResetAfterRespawn(currentRespawnPoint.position);

        health.ResetHealth();

        if(animationController != null)
            animationController.PlayRespawn();

        lifePoints.PrepareForNextLife();

        respawnPending = false;

        Debug.Log($"Player respawnado em: {currentRespawnPoint.name} | " + $"Vida restaurada: {health.CurrentHealth}");
    }

    public void BeginRespawnLock()
    {
        isRespawning = true;

        if(inputReader != null)
            inputReader.SetGamePlayInputBlocked(true);
        
        if(rb != null)
            rb.linearVelocity = Vector2.zero;

        Debug.Log($"[RESPAWN LOCK] Ativado | frame {Time.frameCount}");
    }

    public void FinishRespawnLock()
    {
        if(!isRespawning)
            return;

        if(inputReader != null)
            inputReader.ClearActionRequests();

        isRespawning = false;

        if(inputReader != null)
            inputReader.SetGamePlayInputBlocked(false);

        Debug.Log($"[RESPAWN LOCK] Desativado | frame {Time.frameCount}");
    }

}
