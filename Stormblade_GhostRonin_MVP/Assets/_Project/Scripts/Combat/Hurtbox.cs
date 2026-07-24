using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [Header("Damage Receiver")]
    [SerializeField] private MonoBehaviour damageReceiverBehaviour;

    private IDamageable damageReceiver;

    private void Awake()
    {
        if (damageReceiverBehaviour != null)
        {
            damageReceiver = damageReceiverBehaviour as IDamageable;
        }

        if (damageReceiver == null) 
        {
            damageReceiver = GetComponentInParent<IDamageable>();
        }

        if (damageReceiver == null)
        {
            Debug.LogWarning($"{gameObject.name}: nenhum receptor válido foi encontrado.");
        }

        else
        {
            Debug.Log($"{gameObject.name}: receptor de dano configurado com sucesso.");
        }

    }

    public void ReceiveHit(DamageData damageData)
    {
        if (damageReceiver == null)
        {
            Debug.Log($"{gameObject.name}: damageReceiver está nulo.");
            return;
        }
            
        Debug.LogWarning($"{gameObject.name}: ReceiveHit chamado.");
        damageReceiver.ReceiveDamage(damageData);
    }

}
