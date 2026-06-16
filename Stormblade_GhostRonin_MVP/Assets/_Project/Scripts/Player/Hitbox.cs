using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private Transform damageSource;

    [Header("Hitbox Collider")]
    [SerializeField] private Collider2D hitboxCollider;

    private void Awake()
    {
        if(damageSource == null)
        {
            damageSource = transform.root;
        }

        if (hitboxCollider == null)
        {
            hitboxCollider = GetComponent<Collider2D>();
        }

        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    public void EnableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
            Debug.Log($"{gameObject.name}: hitbox ativada.");
        }
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
            Debug.Log($"{gameObject.name}: hitbox desativada.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if (hurtbox == null)
            return;

        DamageData damageData = new DamageData(damageAmount, damageSource);
        hurtbox.ReceiveHit(damageData);

        Debug.Log($"{gameObject.name}: hitbox atingiu uma hurtbox válida.");
    }
}
