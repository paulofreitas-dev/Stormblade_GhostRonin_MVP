using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class PreplacedEncounterParticipant
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private EncounterResolutionRule resolutionRule = EncounterResolutionRule.DefeatedOnly;

    public GameObject Enemy => enemy;

    public EncounterResolutionRule ResolutionRule => resolutionRule;
}

public class EncounterController : MonoBehaviour
{
    [Header("Preplaced Participants")]
    [SerializeField] private List<PreplacedEncounterParticipant> preplacedParticipants = new();

    [Header("Runtime Debug")]
    [SerializeField] private bool hasActivated;
    [SerializeField] private bool isActive;
    [SerializeField] private List<EncounterParticipant> runtimeParticipants = new();

    [Header("Camera")]
    [SerializeField] private bool lockCameraOnActivate = true;
    [SerializeField] private CameraTargetController cameraTargetController;

    [Header("Spawn Groups")]
    [SerializeField] private List<EnemySpawnGroup> spawnGroups = new();

    [Header("Spawn Runtime")]
    [SerializeField] private int currentSpawnGroupIndex = -1;

    [SerializeField] private List<GameObject> currentSpawnGroupParticipants = new();

    public bool HasActivated => hasActivated;
    public bool IsActive => isActive;

    public IReadOnlyList<EncounterParticipant> RuntimeParticipants => runtimeParticipants;

    public int CurrentSpawnGroupIndex => currentSpawnGroupIndex;

    public IReadOnlyList<GameObject> CurrentSpawnGroupParticipants => currentSpawnGroupParticipants;

    public bool HasMoreSpawnGroups => currentSpawnGroupIndex + 1 < spawnGroups.Count;

    public event Action OnActivated;

    public void ActivateEncounter()
    {
        if(hasActivated)
            return;

        hasActivated = true;
        isActive = true;

        if(lockCameraOnActivate && cameraTargetController != null)
            cameraTargetController.SetEncounterLock(true);

        runtimeParticipants.Clear();

        foreach(PreplacedEncounterParticipant participant in preplacedParticipants)
        {
            if(participant == null)
                continue;

            RegisterParticipant(participant.Enemy, participant.ResolutionRule);
        }

        currentSpawnGroupIndex = -1;
        currentSpawnGroupParticipants.Clear();

        if(spawnGroups.Count > 0)
            SpawnNextGroup();

        Debug.Log($"[ENCOUNTER] {name} ativado. " + $"Participantes iniciais: {runtimeParticipants.Count}.", this);

        OnActivated?.Invoke();
    }

    public bool RegisterParticipant(GameObject enemy, EncounterResolutionRule resolutionRule)
    {
        if(enemy == null)
            return false;

        foreach(EncounterParticipant participant in runtimeParticipants)
        {
            if(participant.Enemy == enemy)
                return false;
        }

        EncounterParticipant newParticipant = new EncounterParticipant(enemy, resolutionRule);

        runtimeParticipants.Add(newParticipant);

        EnemyLifecycle lifecycle = enemy.GetComponent<EnemyLifecycle>();

        if(lifecycle != null)
            lifecycle.OnFinalized += HandleEnemyLifecycleFinalized;

        else
            Debug.LogWarning($"[ENCOUNTER] {enemy.name} não possui EnemyLifecycle.", enemy);

        Debug.Log($"[ENCOUNTER] {enemy.name} registrado em {name}. " + $"com regra {resolutionRule}.", this);

        return true;
    }

    public void ReleaseCameraLock()
    {
        if(!lockCameraOnActivate)
            return;

        if(cameraTargetController == null)
            return;

        cameraTargetController.SetEncounterLock(false);
    }

    public void SpawnNextGroup()
    {
        if (!isActive)
        {
            Debug.LogWarning($"{name}: tentativa de spawn com Encounter inativo.", this);

            return;
        }

        if (!HasMoreSpawnGroups)
        {
            Debug.Log($"[ENCOUNTER] {name}: não existem mais Spawn Groups.", this);

            return;
        }

        currentSpawnGroupIndex++;

        EnemySpawnGroup spawnGroup = spawnGroups[currentSpawnGroupIndex];

        currentSpawnGroupParticipants.Clear();

        if(spawnGroup == null)
        {
            Debug.LogWarning($"{name}: Spawn Group {currentSpawnGroupIndex} não configurado.", this);

            return;
        }

        IReadOnlyList<SpawnedEncounterEnemy> spawnedEnemies = spawnGroup.SpawnAllForEncounter();

        foreach(SpawnedEncounterEnemy spawned in spawnedEnemies)
        {
            if(spawned.Enemy == null)
                continue;

            RegisterParticipant(spawned.Enemy, spawned.ResolutionRule);

            currentSpawnGroupParticipants.Add(spawned.Enemy);
        }

        Debug.Log($"[ENCOUNTER] {name}: Spawn Group " + $"{currentSpawnGroupIndex + 1} iniciado com " + $"{currentSpawnGroupParticipants.Count} inimigo(s).", this);
    }

    private void HandleEnemyLifecycleFinalized(EnemyLifecycle lifecycle,
    EnemyLifecycleResult result)
    {
        if(lifecycle == null)
            return;

        EncounterParticipant participant = FindParticipant(lifecycle.gameObject);

        if(participant == null)
            return;

        bool resolved = participant.TryResolve(result);

        Debug.Log($"[ENCOUNTER] {lifecycle.name} -> {result}. " + $"Rule: {participant.ResolutionRule}. " + $"Resolved: {participant.IsResolved}.", this);

        if (resolved)
        {
            lifecycle.OnFinalized -= HandleEnemyLifecycleFinalized;
        }
    }

    private EncounterParticipant FindParticipant(GameObject enemy)
    {
        if(enemy == null)
            return null;

        foreach(EncounterParticipant participant in runtimeParticipants)
        {
            if(participant == null)
                continue;

            if(participant.Enemy == enemy)
                return participant;
        }
        
        return null;
    }

    [ContextMenu("TEST/Activate Encounter")]
    private void TestActivateEncounter()
    {
        ActivateEncounter();
    }

    [ContextMenu("TEST/Release Camera Lock")]
    private void TestReleaseCameraLock()
    {
        ReleaseCameraLock();
    }

    [ContextMenu("TEST/Spawn Next Group")]
    private void TestSpawnNextGroup()
    {
        SpawnNextGroup();
    }
}
