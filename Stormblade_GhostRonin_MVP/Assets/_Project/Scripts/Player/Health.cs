using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ReceiveDamage(DamageData damageData)
    {
        currentHealth -= damageData.damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log($"{gameObject.name} recebeu {damageData.damageAmount} de dano. Vida atual: {currentHealth}");
    }
}
