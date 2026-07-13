using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour, IPickupCollector
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private PlayerLifePoints lifePoints;

    public Health Health => health;
    public PlayerLifePoints LifePoints => lifePoints;
    public bool CanCollectPickups
    {
        get
        {
            if (health == null)
                return false;

            if (health.IsDead)
                return false;

            return true;
        }
    }

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();

        if (lifePoints == null)
            lifePoints = GetComponent<PlayerLifePoints>();
    }
}
