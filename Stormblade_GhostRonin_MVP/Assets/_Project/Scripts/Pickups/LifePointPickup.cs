using UnityEngine;

public class LifePointPickup : Pickup
{
    [Header("LifePoint Pickup Settings")]
    [SerializeField] private int lifePointAmount = 1;

    protected override void ApplyPickupEffect(IPickupCollector collector)
    {
        if (collector == null)
            return;

        PlayerLifePoints targetLifePoints = collector.LifePoints;

        if (targetLifePoints == null)
        {
            Debug.LogWarning($"{gameObject.name}: coletor não possui PlayerLifePoints.");
        }

        targetLifePoints.AddLifePoint(lifePointAmount);

        Debug.Log($"{gameObject.name}: LifePointPickup coletado. LifePoint somado: {lifePointAmount}");
    }
}
