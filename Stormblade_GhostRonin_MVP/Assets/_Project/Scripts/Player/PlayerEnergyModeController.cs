using UnityEngine;

public class PlayerEnergyModeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader playerInputReader;
    [SerializeField] private PlayerEnergy playerEnergy;
    [SerializeField] private Health health;
    [SerializeField] private LightningRainSpecial lightningRainSpecial;

    private void Awake()
    {
        if (playerInputReader == null)
            playerInputReader = GetComponentInParent<PlayerInputReader>();

        if (playerEnergy == null)
            playerEnergy = GetComponentInParent<PlayerEnergy>();

        if (health == null)
            health = GetComponentInParent<Health>();

        if (lightningRainSpecial == null)
            lightningRainSpecial = GetComponentInChildren<LightningRainSpecial>();
    }

    private void Update()
    {
        HandleSpecialInput();
    }

    private void HandleSpecialInput()
    {
        if (playerInputReader == null)
            return;

        if (!playerInputReader.SpecialRequested)
            return;

        playerInputReader.ConsumeSpecialRequest();

        if(health != null && health.IsDead)
        {
            Debug.Log("Pedido de energia/especial ignorado: player está morto.");
            return;
        }

        if (playerEnergy == null)
            return;

        if (!playerEnergy.IsEnergized)
        {
            TryEnterEnergizedMode();
            return;
        }

        TryUseSpecial();
    }

    private bool IsPlayerDead()
    {
        if (health == null)
            return true;

        return health.IsDead;
    }

    private void TryEnterEnergizedMode()
    {
        if (!playerEnergy.CanEnterEnergized)
        {
            Debug.Log("Não foi possível entrar no modo energizado. A barra precisa estar cheia.");
            return;
        }

        playerEnergy.EnterEnergizedMode();
    }

    private void TryUseSpecial()
    {
        if (!playerEnergy.CanUseSpecial)
        {
            Debug.Log("Não foi possível usar o especial. Sem energia restante.");
            return;
        }

        if (lightningRainSpecial == null)
        {
            Debug.LogWarning("PlayerEnergyModeController: LightningRainSpecial não encontrado.");
            return;
        }

        lightningRainSpecial.Execute();

        playerEnergy.ConsumeAllEnergy();

        Debug.Log("Espcial executado. Energia consumida e modo energizado encerrado.");
    }
}
