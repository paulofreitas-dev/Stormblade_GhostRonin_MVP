using UnityEngine;

public class PlayerLifePoints : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Life Points Settings")]
    [SerializeField] private int initialLifePoints = 3;

    [Header("Life Points State")]
    [SerializeField] private int currentLifePoints;
    [SerializeField] private bool isGameOver;

    public int InitialLifePoints => initialLifePoints;
    public int CurrentLifePoints => currentLifePoints;
    public bool HasLifePoints => currentLifePoints > 0;
    public bool IsGameOver => isGameOver;
    
    private void Awake()
    {
        initialLifePoints = Mathf.Max(1, initialLifePoints);
        currentLifePoints = initialLifePoints;
        isGameOver = false;
    }

    public void AddLifePoint(int amount)
    {
        if (amount <= 0)
            return;

        if (isGameOver)
            return;

        currentLifePoints += amount;

        Debug.Log($"Player recebeu {amount} LifePoint(s). Total atual: {currentLifePoints}");
    }

    public void LoseLifePoint(int amount)
    {
        if (amount <= 0)
            return;

        if (isGameOver)
            return;

        currentLifePoints -= amount;

        if (currentLifePoints < 0)
            currentLifePoints = 0;

        Debug.Log($"Player perdeu {amount} LifePoint(s). Total atual: {currentLifePoints}");

        if (currentLifePoints <= 0)
            TriggerGameOver();

    }

    private void TriggerGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        Debug.Log("Game Over futuro: player ficou sem LifePoints");
    }

    public void ResetLifePoints()
    {
        currentLifePoints = initialLifePoints;
        isGameOver = false;

        Debug.Log($"LifePoints resetados. Total atual: {currentLifePoints}");
    }
}
