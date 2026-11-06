using UnityEngine;
using System;
using System.Collections.Generic;

public class EncounterController : MonoBehaviour
{
    [Header("Preplaced Participants")]
    [SerializeField] private List<GameObject> preplacedParticipants = new();

    [Header("Runtime Debug")]
    [SerializeField] private bool hasActivated;
    [SerializeField] private bool isActive;

    [SerializeField] private List<GameObject> runtimeParticipants = new();

    public bool HasActivated => HasActivated;
    public bool IsActive => isActive;

    public IReadOnlyList<GameObject> RuntimeParticipants => runtimeParticipants;

    public event Action OnActivated;

    public void ActivateEncounter()
    {
        if(hasActivated)
            return;

        hasActivated = true;
        isActive = true;

        runtimeParticipants.Clear();

        foreach(GameObject participant in preplacedParticipants)
        {
            RegisterParticipant(participant);
        }

        Debug.Log($"[ENCOUNTER] {name} ativado. " + $"Participantes iniciais: {runtimeParticipants.Count}.", this);

        OnActivated?.Invoke();
    }

    public bool RegisterParticipant(GameObject participant)
    {
        if(participant == null)
            return false;

        if(runtimeParticipants.Contains(participant))
            return false;

        runtimeParticipants.Add(participant);

        Debug.Log($"[ENCOUNTER] {participant.name} registrado em {name}.", this);

        return true;
    }

    [ContextMenu("TEST/Activate Encounter")]
    private void TestActivateEncounter()
    {
        ActivateEncounter();
    }
}
