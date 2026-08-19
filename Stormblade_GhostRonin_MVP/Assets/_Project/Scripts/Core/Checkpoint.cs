using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    public Transform RespawnPoint => respawnPoint;

    public void Active(PlayerRespawnController playerRespawn)
    {
        if(playerRespawn == null)
            return;

        playerRespawn.SetCheckpoint(respawnPoint);
    }
}
