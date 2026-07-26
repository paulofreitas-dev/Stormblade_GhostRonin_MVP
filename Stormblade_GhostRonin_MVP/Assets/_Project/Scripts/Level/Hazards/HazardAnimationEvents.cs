using UnityEngine;

public class HazardAnimationEvents : MonoBehaviour
{
    [SerializeField] private HazardDamage hazardDamage;

    public void BeginLoop()
    {
        if(hazardDamage == null)
            return;

        hazardDamage.BeginLoop();
    }

    public void ActivateHitbox()
    {
        if(hazardDamage == null)
            return;

        hazardDamage.ActivateHitbox();
    }

}
