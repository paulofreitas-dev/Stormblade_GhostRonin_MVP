using UnityEngine;

public interface IPickupCollector
{
    bool CanCollectPickups { get; }
    Health Health { get; }
    PlayerLifePoints LifePoints { get; }
}

public class Pickup : MonoBehaviour
{
    [Header("Pickup Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Pickup Visual")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D pickupCollider;

    [Header("Pickup State")]
    [SerializeField] private bool wasCollected;
    [SerializeField] private bool isDestroying;

    private static readonly int DestroyHash = Animator.StringToHash("Destroy");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (pickupCollider == null)
            pickupCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        wasCollected = false;
        isDestroying = false;

        if (pickupCollider != null)
            pickupCollider.enabled = true;

        if (lifeTime > 0f)
        {
            Invoke(nameof(Expire), lifeTime);
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Expire));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasCollected)
            return;

        if (isDestroying)
            return;

        IPickupCollector collector = collision.GetComponentInParent<IPickupCollector>();

        if (collector == null)
            return;

        if (!collector.CanCollectPickups)
            return;

        Collect(collector);
    }

    private void Collect(IPickupCollector collector)
    {
        wasCollected = true;

        CancelInvoke(nameof(Expire));

        ApplyPickupEffect(collector);

        StartDestroySequence();
    }

    protected virtual void ApplyPickupEffect(IPickupCollector collector)
    {
        Debug.Log($"{gameObject.name}: pickup coletado, mas nenhum efeito específico foi definido.");
    }

    private void Expire()
    {
        if (wasCollected)
            return;

        if (isDestroying)
            return;

        Debug.Log($"{gameObject.name}: pickup expirou");

        StartDestroySequence();
    }

    private void StartDestroySequence()
    {
        if (isDestroying)
            return;

        isDestroying = true;

        if (pickupCollider != null)
            pickupCollider.enabled = false;

        if(animator != null)
        {
            animator.SetTrigger(DestroyHash);
            return;
        }

        DestroyAfterAnimation();
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
