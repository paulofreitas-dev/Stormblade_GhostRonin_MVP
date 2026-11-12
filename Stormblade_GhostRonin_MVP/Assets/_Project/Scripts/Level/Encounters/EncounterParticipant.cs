using UnityEngine;

[System.Serializable]
public class EncounterParticipant
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private EncounterResolutionRule resolutionRule = EncounterResolutionRule.DefeatedOnly;

    [SerializeField] private bool isResolved;

    public GameObject Enemy => enemy;

    public EncounterResolutionRule ResolutionRule => resolutionRule;

    public bool IsResolved => isResolved;

    public EncounterParticipant(GameObject enemy, EncounterResolutionRule resolutionRule)
    {
        this.enemy = enemy;
        this.resolutionRule = resolutionRule;
        isResolved = false;
    }

    public bool TryResolve(EnemyLifecycleResult lifecycleResult)
    {
        if(isResolved)
            return false;

        if(lifecycleResult == EnemyLifecycleResult.Defeated)
        {
            isResolved = true;
            return true;
        }
        
        if(lifecycleResult == EnemyLifecycleResult.Escaped && resolutionRule == EncounterResolutionRule.DefeatedOrEscaped)
        {
            isResolved = true;
            return true;
        }

        return false;

    }
}
