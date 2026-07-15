using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startEnergy = 0;

    [Header("Energy State")]
    [SerializeField] private int currentEnergy;

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;
    public bool IsFull => currentEnergy >= maxEnergy;
    public bool IsEmpty => currentEnergy <= 0;

    public float NormalizedEnergy
    {
        get
        {
            if (maxEnergy <= 0)
                return 0f;

            return (float)currentEnergy / maxEnergy;
        }
    }

    private void Awake()
    {
        maxEnergy = Mathf.Max(1, maxEnergy);
        startEnergy = Mathf.Clamp(startEnergy, 0, maxEnergy);
        currentEnergy = startEnergy;
    }

    public int AddEnergy(int amount)
    {
        if (amount <= 0)
            return 0;

        if (IsFull)
        {
            Debug.Log($"Energia já está cheia. Energia atual: {currentEnergy}/{maxEnergy}");
            return 0;
        }

        int previousEnergy = currentEnergy;

        currentEnergy += amount;

        if (currentEnergy > maxEnergy)
            currentEnergy = maxEnergy;

        int addedEnergy = currentEnergy - previousEnergy;

        Debug.Log($"Player recebeu {addedEnergy} de energia. Energia atual: {currentEnergy}/{maxEnergy}");

        return addedEnergy;
    }
}
