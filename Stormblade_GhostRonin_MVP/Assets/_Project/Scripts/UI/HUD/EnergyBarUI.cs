using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if(playerEnergy != null)
            playerEnergy.OnEnergyChanged += UpdateEnergyBar;
    }

    private void Start()
    {
        RefreshEnergyBar();
    }

    private void OnDisable()
    {
        if(playerEnergy != null)
            playerEnergy.OnEnergyChanged -= UpdateEnergyBar;
    }

    private void RefreshEnergyBar()
    {
        if(playerEnergy == null)
            return;

        UpdateEnergyBar(playerEnergy.CurrentEnergy, playerEnergy.MaxEnergy);
    }

    private void UpdateEnergyBar(int currentEnergy, int maxEnergy)
    {
        if(fillImage == null)
            return;
        
        if(maxEnergy <= 0)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        float normalizedEnergy = (float)currentEnergy / maxEnergy;

        fillImage.fillAmount = Mathf.Clamp01(normalizedEnergy);
    }

    
}
