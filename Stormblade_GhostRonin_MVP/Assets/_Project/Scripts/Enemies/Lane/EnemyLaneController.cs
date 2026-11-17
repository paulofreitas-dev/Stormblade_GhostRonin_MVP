using UnityEngine;
using System;

public class EnemyLaneController : MonoBehaviour, IEnemySpawnLaneConfigurable, IGameplayLaneProvider
{
    [Header("Initial State")]
    [SerializeField] private GameplayLane startingLane = GameplayLane.Front;

    [Header("References")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Runtime Debug")]
    [SerializeField] private GameplayLane currentLane;
    [SerializeField] private GameplayLane targetLane;
    [SerializeField] private bool isTransitioning;

    private int frontLaneBodyLayer;
    private int backLaneBodyLayer;

    public GameplayLane CurrentLane => currentLane;
    public GameplayLane TargetLane => targetLane;
    public bool IsTransitioning => isTransitioning;

    public event Action<GameplayLane, GameplayLane> OnLaneChanged;

    private void Awake()
    {
        CacheLayers();

        currentLane = startingLane;
        targetLane = currentLane;
        isTransitioning = false;

        ApplyBodyLayer(currentLane);
    }

    private void CacheLayers()
    {
        frontLaneBodyLayer = LayerMask.NameToLayer("FrontLaneBody");

        backLaneBodyLayer = LayerMask.NameToLayer("BackLaneBody");

        if(frontLaneBodyLayer == -1)
            Debug.LogError("EnemyLaneController: FrontLaneBody não encontrada.", this);

        if(backLaneBodyLayer == -1)
            Debug.LogError("EnemyLaneController: BackLaneBody não encontrada.", this);
    }

    public GameplayLane GetOppositeLane()
    {
        return currentLane == GameplayLane.Front ? GameplayLane.Back : GameplayLane.Front;
    }

    public bool BeginTransition(GameplayLane newTargetLane)
    {
        if(isTransitioning)
            return false;

        if(newTargetLane == currentLane)
            return false;

        targetLane = newTargetLane;
        isTransitioning = true;

        Debug.Log($"[ENEMY LANE] {name}: transição iniciada. " + 
        $"{currentLane} -> {targetLane}.",
        this);

        return true;
    }

    public void SetLane(GameplayLane newLane)
    {
        GameplayLane previousLane = currentLane;

        currentLane = newLane;
        targetLane = newLane;
        isTransitioning = false;

        ApplyBodyLayer(currentLane);

        if(previousLane == currentLane)
            return;

        OnLaneChanged?.Invoke(previousLane, currentLane);

        Debug.Log($"[ENEMY LANE] {name}: " +
        $"{previousLane} -> {currentLane}",
        this);
    }

    private void ApplyBodyLayer(GameplayLane lane)
    {
        if(bodyCollider == null)
        {
            Debug.LogWarning("EnemyLaneController: BodyCollider não configurado.", this);

            return;
        }

        int targetLayer = lane == GameplayLane.Front ? frontLaneBodyLayer : backLaneBodyLayer;

        if(targetLayer == -1)
            return;

        bodyCollider.gameObject.layer = targetLayer;
    }

    public void SetInitialLane(GameplayLane lane)
    {
        currentLane = lane;
        targetLane = lane;
        isTransitioning = false;

        ApplyBodyLayer(currentLane);
    }

    public void CompleteTransition()
    {
        if(!isTransitioning)
            return;

        GameplayLane previousLane = currentLane;

        currentLane = targetLane;
        isTransitioning = false;

        ApplyBodyLayer(currentLane);

        OnLaneChanged?.Invoke(previousLane, currentLane);

        Debug.Log($"[ENEMY LANE] {name}: transição concluída. " +
        $"{previousLane} -> {currentLane}.",
        this);
    }

    public void CancelTransition()
    {
        if(!isTransitioning)
            return;

        targetLane = currentLane;
        isTransitioning = false;

        ApplyBodyLayer(currentLane);

        Debug.Log($"[ENEMY LANE] {name}: transição cancelada. " + 
        $"Lane atual: {currentLane}.", 
        this);
    }
}
