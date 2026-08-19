using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Respawn Points")]
    [SerializeField] private Transform initialRespawnPoint;
    [SerializeField] private Transform currentRespawnPoint;

    [Header("Respawn State")]
    [SerializeField] private bool respawnPending;

    public Transform CurrentRespawnPoint => currentRespawnPoint;
    public bool RespawnPending => respawnPending;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();

        if(initialRespawnPoint == null)
        {
            Debug.LogWarning("PlayerRespawnController: Initial Respawn Point não configurado.");
            return;
        }

        currentRespawnPoint = initialRespawnPoint;
        respawnPending = false;
    }

    private void OnEnable()
    {
        if(health != null)
            health.OnDied += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        if(health != null)
            health.OnDied -= HandlePlayerDeath;
    }

    public void SetCheckpoint(Transform newRespawnPoint)
    {
        if(newRespawnPoint == null)
            return;

        currentRespawnPoint = newRespawnPoint;

        Debug.Log($"Checkpoint atualizado: {newRespawnPoint.name}");
    }

    private void HandlePlayerDeath()
    {
        if(respawnPending)
            return;

        respawnPending = true;

        if(currentRespawnPoint == null)
        {
            Debug.LogWarning("PlayerRespawnController: morte detectada, mas não existe Respawn Point.");
        }

        Debug.Log($"PlayerRespawnController: morte detectada. " + $"Respawn pendente em: {currentRespawnPoint.name}");
    }
}
