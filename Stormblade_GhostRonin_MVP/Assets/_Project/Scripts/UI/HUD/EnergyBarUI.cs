using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Image fillImage;

    [Header("Full Energy Visual")]
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashInterval = 0.15f;

    private float flashTimer;
    private bool useFlashColor;

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

    private void Update()
    {
        UpdateEnergyFlash();
    }

    private void UpdateEnergyFlash()
    {
        if(playerEnergy == null || fillImage == null)
            return;

        if (!playerEnergy.IsFull)
        {
            fillImage.color = normalColor;
            flashTimer = 0f;
            useFlashColor = false;
            return;
        }

        flashTimer += Time.deltaTime;

        if(flashTimer < flashInterval)
            return;

        flashTimer = 0f;
        useFlashColor = !useFlashColor;

        fillImage.color = useFlashColor ? flashColor : normalColor;
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
