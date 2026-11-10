using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EncounterTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EncounterController encounterController;

    [Header("Settings")]
    [SerializeField] private bool disableAfterActivation = true;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();

        if(triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(encounterController == null)
            return;

        PlayerMovement player = other.GetComponentInParent<PlayerMovement>();

        if(player == null)
            return;

        encounterController.ActivateEncounter();

        if(disableAfterActivation && encounterController.HasActivated && triggerCollider != null)
            triggerCollider.enabled = false;
    }
}
