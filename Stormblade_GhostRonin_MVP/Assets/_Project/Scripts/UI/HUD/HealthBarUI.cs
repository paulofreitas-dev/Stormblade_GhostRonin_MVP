using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if(health != null)
            health.OnHealthChanged += UpdateHealthBar;
    }

    void Start()
    {
        RefreshHealthBar();
    }

    private void OnDisable()
    {
        if(health != null)
            health.OnHealthChanged -= UpdateHealthBar;
    }

    private void RefreshHealthBar()
    {
        if(health == null)
            return;

        UpdateHealthBar(health.CurrentHealth, health.MaxHealth);
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if(fillImage == null)
            return;

        if(maxHealth <= 0)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        float normalizedHealth = (float)currentHealth / maxHealth;

        fillImage.fillAmount = Mathf.Clamp01(normalizedHealth);
    }
}
