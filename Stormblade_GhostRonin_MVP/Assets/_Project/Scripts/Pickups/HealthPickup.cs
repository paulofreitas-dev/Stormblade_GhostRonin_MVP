using UnityEngine;

public class HealthPickup : Pickup
{
    [Header("Health Pickup Settings")]
    [SerializeField] private int healAmount = 1;

    protected override void ApplyPickupEffect(IPickupCollector collector)
    {
        if (collector == null)
            return;

        Health targetHealth = collector.Health;

        if (targetHealth == null)
        {
            Debug.LogWarning($"{gameObject.name}: coletor não possui Health");
            return;
        }

        targetHealth.Heal(healAmount);

        Debug.Log($"{gameObject.name}: HealthPickup coletado. Curada tentada: {healAmount}");
    }
}
