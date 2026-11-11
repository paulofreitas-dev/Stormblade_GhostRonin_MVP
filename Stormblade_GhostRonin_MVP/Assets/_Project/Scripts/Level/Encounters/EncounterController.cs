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

        IReadOnlyList<GameObject> spawnedEnemies = spawnGroup.SpawnAll();

        foreach(GameObject enemy in spawnedEnemies)
        {
            if(enemy == null)
                continue;

            RegisterParticipant(enemy, EncounterResolutionRule.DefeatedOrEscaped);

            currentSpawnGroupParticipants.Add(enemy);
        }

        Debug.Log($"[ENCOUNTER] {name}: Spawn Group " + $"{currentSpawnGroupIndex + 1} iniciado com " + $"{currentSpawnGroupParticipants.Count} inimigo(s).", this);
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
