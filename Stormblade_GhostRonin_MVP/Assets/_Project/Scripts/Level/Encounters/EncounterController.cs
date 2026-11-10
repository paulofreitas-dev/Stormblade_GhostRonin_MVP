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

    [Header("Camera")]
    [SerializeField] private bool lockCameraOnActivate = true;
    [SerializeField] private CameraTargetController cameraTargetController;

    public bool HasActivated => hasActivated;
    public bool IsActive => isActive;

    public IReadOnlyList<GameObject> RuntimeParticipants => runtimeParticipants;

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

    public void ReleaseCameraLock()
    {
        if(!lockCameraOnActivate)
            return;

        if(cameraTargetController == null)
            return;

        cameraTargetController.SetEncounterLock(false);
    }

    [ContextMenu("TEST/Activate Encounter")]
    private void TestActivateEncounter()
    {
        ActivateEncounter();
    }

    [ContextMenu("TEST/Release Camera Lock")]
    private void TesteReleaseCameraLock()
    {
        ReleaseCameraLock();
    }
}
