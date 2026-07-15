using UnityEngine;

public class EnergyPickup : Pickup
{
    [Header("Energy Pickup Settings")]
    [SerializeField] private int energyAmount = 1;

    protected override void ApplyPickupEffect(IPickupCollector collector)
    {
        if (collector == null)
            return;

        PlayerEnergy targetEnergy = collector.Energy;

        if(targetEnergy == null)
        {
            Debug.LogWarning($"{gameObject.name}: coletor não possui PlayerEnergy.");
            return;
        }

        int addedEnergy = targetEnergy.AddEnergy(energyAmount);

        Debug.Log($"{gameObject.name}: EnergyPickup coletado. Energia tentada: {energyAmount}. Energia adicionada: {addedEnergy}");
    }
}
