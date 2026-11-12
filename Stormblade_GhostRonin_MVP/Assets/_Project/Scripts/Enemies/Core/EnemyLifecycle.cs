using UnityEngine;
using System;

public enum EnemyLifecycleResult
{
    Defeated,
    Escaped
}

public class EnemyLifecycle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;

    [Header("Runtime Debug")]
    [SerializeField] private bool isFinalized;
    [SerializeField] private EnemyLifecycleResult finalResult;

    public bool IsFinalized => isFinalized;
    public EnemyLifecycleResult FinalResult => finalResult;

    public event Action<EnemyLifecycle, EnemyLifecycleResult> OnFinalized;

    private void Awake()
    {
        if(health == null)
            health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if(health != null)
            health.OnDied += HandleDefeated;
    }

    private void OnDisable()
    {
        if(health != null)
            health.OnDied -= HandleDefeated;
    }

    private void HandleDefeated()
    {
        FinalizeLifecycle(EnemyLifecycleResult.Defeated);
    }

    public void NotifyEscaped()
    {
        FinalizeLifecycle(EnemyLifecycleResult.Escaped);
    }

    private void FinalizeLifecycle(EnemyLifecycleResult result)
    {
        if(isFinalized)
            return;

        isFinalized = true;
        finalResult = result;

        Debug.Log($"[ENEMY LIFECYCLE] {name}: {result}.", this);

        OnFinalized?.Invoke(this, result);
    }
}
