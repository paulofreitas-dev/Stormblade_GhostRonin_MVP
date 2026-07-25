using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public bool IsAlive => !isDead;

    public event Action OnDied;

    private void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        isDead = false;
    }

    public void ReceiveDamage(DamageData damageData)
    {
        if (isDead)
            return;

        currentHealth -= damageData.damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} morreu.");

            OnDied?.Invoke();
        }

        Debug.Log($"{gameObject.name} recebeu {damageData.damageAmount} de dano. Vida atual: {currentHealth}");
    }

    public void Heal(int healAmount)
    {
        if (isDead)
            return;

        if (healAmount <= 0)
            return;

        int previousHealth = currentHealth;

        currentHealth += healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        int healedAmount = currentHealth - previousHealth;

        Debug.Log($"{gameObject.name} recuperou {healedAmount} de vida. Vida atual: {currentHealth}");
    }
}
