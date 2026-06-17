using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private Transform damageSource;

    [Header("Hitbox Collider")]
    [SerializeField] private Collider2D hitboxCollider;

    private HashSet<Hurtbox> hitHurtboxes = new HashSet<Hurtbox>();

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
        hitHurtboxes.Clear();

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

        if (damageSource != null && hurtbox.transform.root == damageSource.root)
        {
            Debug.Log($"{gameObject.name}: autoacerto ignorado.");
            return;
        }

        if (hitHurtboxes.Contains(hurtbox))
        {
            Debug.Log($"{gameObject.name}: hurtbox já atingida nesta ativação");
            return;
        }

        hitHurtboxes.Add(hurtbox);

        DamageData damageData = new DamageData(damageAmount, damageSource);
        hurtbox.ReceiveHit(damageData);

        Debug.Log($"{gameObject.name}: hitbox atingiu uma hurtbox válida.");
    }
}
