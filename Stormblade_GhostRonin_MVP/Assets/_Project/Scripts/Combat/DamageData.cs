using UnityEngine;

public struct DamageData
{
    public int damageAmount;
    public Transform sourceTransform;

    public DamageData(int damageAmount, Transform sourceTransform)
    {
        this.damageAmount = damageAmount;
        this.sourceTransform = sourceTransform;
    }

}
