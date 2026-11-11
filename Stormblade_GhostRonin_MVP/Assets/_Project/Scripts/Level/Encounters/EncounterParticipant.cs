using UnityEngine;

[System.Serializable]
public class EncounterParticipant : MonoBehaviour
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

    public void MarkResolved()
    {
        isResolved = true;
    }
}
