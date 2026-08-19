using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private Checkpoint checkpoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerRespawnController playerRespawn = other.GetComponentInParent<PlayerRespawnController>();

        if(playerRespawn == null)
            return;

        checkpoint.Active(playerRespawn);
    }
}
