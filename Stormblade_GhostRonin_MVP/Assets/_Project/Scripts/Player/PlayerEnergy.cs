using System;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy Settings")]
    [SerializeField] private int maxEnergy = 10;
    [SerializeField] private int startEnergy = 0;

    [Header("Energized Mode Settings")]
    [SerializeField] private float energyDrainPerSecond = 1f;

    [Header("Energy State")]
    [SerializeField] private int currentEnergy;
    [SerializeField] private bool isEnergized;

    private float drainAccumulator;

    public event Action<int, int> OnEnergyChanged;

    public int CurrentEnergy => currentEnergy;
    public int MaxEnergy => maxEnergy;

    public bool IsFull => currentEnergy >= maxEnergy;
    public bool IsEmpty => currentEnergy <= 0;

    public bool IsEnergized => isEnergized;
    public bool CanEnterEnergized => IsFull & !isEnergized;
    public bool CanUseSpecial => isEnergized && currentEnergy > 0;

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

        isEnergized = false;
        drainAccumulator = 0f;
    }

    private void Update()
    {
        if (!isEnergized)
            return;

        DrainEnergyOverTime();
    }

    private void LateUpdate() 
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EnterEnergizedMode();
        }
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

        if(addedEnergy <= 0)
            return 0;

        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        Debug.Log($"Player recebeu {addedEnergy} de energia. Energia atual: {currentEnergy}/{maxEnergy}");

        return addedEnergy;
    }

    public bool EnterEnergizedMode()
    {
        if(!CanEnterEnergized)
        {
            Debug.Log("Não foi possível entrar no modo energizado.");
            return false;
        }

        isEnergized = true;
        drainAccumulator = 0f;

        Debug.Log("Player entrou no modo energizado.");

        return true;
    }

    public void ExitEnergizedMode()
    {
        if (!isEnergized)
            return;

        isEnergized = false;
        drainAccumulator = 0f;

        Debug.Log("Player saiu do modo energizado.");
    }

    public int ConsumeEnergy(int amount)
    {
        if (amount <= 0)
            return 0;

        if (IsEmpty)
            return 0;

        int previousEnergy = currentEnergy;

        currentEnergy -= amount;

        if (currentEnergy < 0)
            currentEnergy = 0;

        int consumedEnergy = previousEnergy - currentEnergy;

        if(consumedEnergy <= 0)
            return 0;

        OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        Debug.Log($"Player consumiu {consumedEnergy} de energia. Energia atual: {currentEnergy}/{maxEnergy}");

        if (currentEnergy <= 0 && isEnergized)
        {
            ExitEnergizedMode();
        }

        return consumedEnergy;
    }

    public int ConsumeAllEnergy()
    {
        return ConsumeEnergy(currentEnergy);
    }
    
    private void DrainEnergyOverTime()
    {
        if (energyDrainPerSecond <= 0f)
            return;

        drainAccumulator += energyDrainPerSecond * Time.deltaTime;

        if (drainAccumulator < 1f)
            return;

        int energyToDrain = Mathf.FloorToInt(drainAccumulator);
        drainAccumulator -= energyToDrain;

        ConsumeEnergy(energyToDrain);
    }
}
