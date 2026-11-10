using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnGroup : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private List<EnemySpawnPoint> spawnPoints = new();

    [Header("Runtime Debug")]
    [SerializeField] private bool hasSpawned;

    [SerializeField] private List<GameObject> spawnedEnemies = new();

    public bool HasSpawned => hasSpawned;

    public IReadOnlyList<GameObject> SpawnedEnemies => spawnedEnemies;

    public IReadOnlyList<GameObject> SpawnAll()
    {
        if (hasSpawned)
        {
            Debug.LogWarning($"{name}: este Spawn Group já foi executado.", this);

            return spawnedEnemies;
        }

        hasSpawned = true;
        spawnedEnemies.Clear();

        foreach(EnemySpawnPoint spawnPoint in spawnPoints)
        {
            if(spawnPoint == null)
                continue;

            GameObject spawnedEnemy = spawnPoint.Spawn();

            if(spawnedEnemy == null)
                continue;

            spawnedEnemies.Add(spawnedEnemy);
        }

        Debug.Log($"[SPAWN GROUP] {name} criou " + $"{spawnedEnemies.Count} inimigo(s).", this);

        return spawnedEnemies;
    }

    [ContextMenu("TEST/Spawn Group")]
    private void TestSpawnGroup()
    {
        SpawnAll();
    }
}
