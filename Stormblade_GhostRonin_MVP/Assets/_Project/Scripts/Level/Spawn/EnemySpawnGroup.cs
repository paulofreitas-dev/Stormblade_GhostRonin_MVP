using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnGroup : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private List<EnemySpawnPoint> spawnPoints = new();

    [Header("Runtime Debug")]
    [SerializeField] private bool hasSpawned;

    [SerializeField] private List<GameObject> spawnedEnemies = new();

    private List<SpawnedEncounterEnemy> spawnedEncounterEnemies = new();

    public bool HasSpawned => hasSpawned;

    public IReadOnlyList<GameObject> SpawnedEnemies => spawnedEnemies;

    // --------------------------------
    // SPAWN NORMAL / TESTE
    // --------------------------------

    public IReadOnlyList<GameObject> SpawnAll()
    {
        SpawnIfNeeded();

        return spawnedEnemies;
    }

    // --------------------------------
    // SPAWN PARA ENCOUNTER
    // --------------------------------

    public IReadOnlyList<SpawnedEncounterEnemy> SpawnAllForEncounter()
    {
        SpawnIfNeeded();

        return spawnedEncounterEnemies;
    }

    // --------------------------------
    // SPAWN INTERNO
    // --------------------------------

    private void SpawnIfNeeded()
    {
        if (hasSpawned)
        {
            Debug.LogWarning($"{name}: este Spawn Group já foi executado.", this);

            return;
        }

        hasSpawned = true;

        spawnedEnemies.Clear();
        spawnedEncounterEnemies.Clear();

        foreach(EnemySpawnPoint spawnPoint in spawnPoints)
        {
            if(spawnPoint == null)
                continue;

            GameObject spawnedEnemy = spawnPoint.Spawn();

            if(spawnedEnemy == null)
                continue;

            spawnedEnemies.Add(spawnedEnemy);

            SpawnedEncounterEnemy encounterEnemy = new SpawnedEncounterEnemy(spawnedEnemy, spawnPoint.EncounterResolutionRule);

            spawnedEncounterEnemies.Add(encounterEnemy);
        }

        Debug.Log($"[SPAWN GROUP] {name} criou " + $"{spawnedEnemies.Count} inimigo(s).", this);
    }

    // --------------------------------
    // TEST
    // --------------------------------

    [ContextMenu("TEST/Spawn Group")]
    private void TestSpawnGroup()
    {
        SpawnAll();
    }
}
