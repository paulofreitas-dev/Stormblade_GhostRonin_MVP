using UnityEngine;
using System.Collections.Generic;

public class LightningRainSpecial : MonoBehaviour
{
    [Header("Special Damage")]
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private LayerMask targetLayers;

    [Header("Area Settings")]
    [SerializeField] private Transform areaOrigin;
    [SerializeField] private Vector2 areaOffset = new Vector2(0f, 1.5f);
    [SerializeField] private Vector2 areaSize = new Vector2(12f, 6f);

    [Header("Optional Visual Proxy")]
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private float visualLifetime = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool isExecuting;
    [SerializeField] private bool showDebugArea = true;

    private IDamageable ownerDamageable;

    public bool IsExecuting => isExecuting;

    private void Awake()
    {
        if (areaOrigin == null)
            areaOrigin = transform;

        ownerDamageable = GetComponentInParent<IDamageable>();
    }

    public void Execute()
    {
        if (isExecuting)
            return;

        isExecuting = true;

        SpawnVisualProxy();
        ApplyDamageInArea();

        isExecuting = false;
    }

    private void ApplyDamageInArea()
    {
        Vector2 center = GetAreaCenter();

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, areaSize, 0f, targetLayers);

        HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

        DamageData damageData = new DamageData();
        damageData.damageAmount = damageAmount;

        foreach (Collider2D hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();

            if (target == null)
                continue;

            if (target == ownerDamageable)
                continue;

            if (damagedTargets.Contains(target))
                continue;

            damagedTargets.Add(target);
            target.ReceiveDamage(damageData);
        }

        Debug.Log($"LightningRainSpecial executado. Alvos atingidos: {damagedTargets.Count}");
    }

    private void SpawnVisualProxy()
    {
        if (visualPrefab == null)
            return;

        GameObject visualInstance = Instantiate(visualPrefab, GetAreaCenter(), Quaternion.identity);
        Destroy(visualInstance, visualLifetime);
    }

    private Vector2 GetAreaCenter()
    {
        Transform origin = areaOrigin != null ? areaOrigin : transform;
        return (Vector2)origin.position + areaOffset;
    }

    [ContextMenu("Test Execute Lightning Rain")]
    private void TestExecuteLightningRain()
    {
        Execute();
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugArea)
            return;

        Gizmos.DrawWireCube(GetAreaCenter(), areaSize);
    }
} 
