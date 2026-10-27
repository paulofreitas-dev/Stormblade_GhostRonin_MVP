using UnityEngine;
using System;

public class EnemyLaneController : MonoBehaviour
{
    [Header("Initial State")]
    [SerializeField] private GameplayLane startingLane = GameplayLane.Front;

    [Header("References")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Runtime Debug")]
    [SerializeField] private GameplayLane currentLane;

    private int frontLaneBodyLayer;
    private int backLaneBodyLayer;

    public GameplayLane CurrentLane => currentLane;

    public event Action<GameplayLane, GameplayLane> OnLaneChanged;

    private void Awake()
    {
        CacheLayers();

        currentLane = startingLane;

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

    public void SetLane(GameplayLane newLane)
    {
        if(newLane == currentLane)
        {
            ApplyBodyLayer(currentLane);
            return;
        }

        GameplayLane previousLane = currentLane;

        currentLane = newLane;

        ApplyBodyLayer(currentLane);

        OnLaneChanged?.Invoke(previousLane, currentLane);

        Debug.Log($"[ENEMY LANE] {name}: " + $"{previousLane} -> {currentLane}", this);
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
}
