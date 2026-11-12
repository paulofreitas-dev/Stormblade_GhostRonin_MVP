using UnityEngine;

public struct SpawnedEncounterEnemy
{
    public GameObject Enemy { get; }

    public EncounterResolutionRule ResolutionRule{ get; }

    public SpawnedEncounterEnemy(GameObject enemy, EncounterResolutionRule resolutionRule)
    {
        Enemy = enemy;
        ResolutionRule = resolutionRule;
    }
}
