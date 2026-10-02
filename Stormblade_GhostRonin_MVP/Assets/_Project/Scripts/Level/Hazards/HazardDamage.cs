using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    public enum HazardActivationMode
    {
        Persistent,
        AnimationDriven
    }

    [Header("Activation")]
    [SerializeField] private HazardActivationMode activationMode = HazardActivationMode.AnimationDriven;

    [Header("References")]
    [SerializeField] private Hitbox hitbox;

    private void Awake()
    {
        if(hitbox == null)
        {
            Debug.LogWarning($"{gameObject.name}: a hitbox do hazard não foi configurada.");
            return;
        }

        ApplyInitialState();
    }

    private void ApplyInitialState()
    {
        if(activationMode == HazardActivationMode.Persistent)
        {
            hitbox.EnableHitbox();
            return;
        }

        hitbox.DisableHitbox();
    }

    public void BeginLoop()
    {
        if(activationMode != HazardActivationMode.AnimationDriven)
            return;

        DeactivateHitbox();
    }

    public void ActivateHitbox()
    {
        if(hitbox == null)
            return;

        hitbox.EnableHitbox();
    }

    public void DeactivateHitbox()
    {
        if(hitbox == null)
            return;

        if(activationMode == HazardActivationMode.Persistent)
            return;

        hitbox.DisableHitbox();
    }
}
