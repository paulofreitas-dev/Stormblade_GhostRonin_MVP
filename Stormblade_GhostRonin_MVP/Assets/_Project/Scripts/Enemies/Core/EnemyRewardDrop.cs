using UnityEngine;

public class EnemyRewardDrop : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    [Header("Runtime Debug")]
    [SerializeField] private bool rewardDropped;

    public bool RewardDropped => rewardDropped;

    public void SpawnReward()
    {
        if(rewardDropped)
            return;

        rewardDropped = true;

        if(rewardPrefab == null)
        {
            Debug.LogWarning($"{name}: Reward Prefab não configurado.", this);

            return;
        }

        Vector3 spawnPosition = rewardSpawnPoint != null ? rewardSpawnPoint.position : transform.position;

        Instantiate(rewardPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"[ENEMY REWARD] {name} gerou recompensa.", this);
    }
}
