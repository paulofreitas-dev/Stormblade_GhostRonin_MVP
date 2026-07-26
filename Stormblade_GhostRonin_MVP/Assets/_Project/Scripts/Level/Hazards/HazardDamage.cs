using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    [SerializeField] private Hitbox hitbox;

    private void Awake()
    {
        if(hitbox == null)
        {
            Debug.LogWarning($"{gameObject.name}: a hitbox do hazard não foi configurada.");
            return;
        }

        hitbox.DisableHitbox();
    }

    public void BeginLoop()
    {
        hitbox.DisableHitbox();
    }

    public void ActivateHitbox()
    {
        hitbox.EnableHitbox();
    }
}
