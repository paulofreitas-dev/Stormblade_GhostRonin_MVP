using UnityEngine;
using System;

public class PlayerLaneController : MonoBehaviour
{
    [Header("Initial State")]
    [SerializeField] private GameplayLane startingLane = GameplayLane.Front;

    [Header("References")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Runtime Debug")]
    [SerializeField] private GameplayLane currentLane;
    [SerializeField] private GameplayLane targetLane;
    [SerializeField] private bool isTransitioning;

    [Header("Ground Detection")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LayerMask frontGroundMask;
    [SerializeField] private LayerMask backGroundMask;

    private int frontLaneBodyLayer;
    private int backLaneBodyLayer;
    private int laneTransitionBodyLayer;

    public GameplayLane CurrentLane => currentLane;
    public GameplayLane TargetLane => targetLane;
    public bool IsTransitioning => isTransitioning;

    public event Action<GameplayLane, GameplayLane> OnLaneChanged;

    private void Awake()
    {
        if(playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        CacheLayers();

        currentLane = startingLane;
        targetLane = currentLane;
        isTransitioning = false;

        ApplyLaneSettings(currentLane);
    }

    private void CacheLayers()
    {
        frontLaneBodyLayer = LayerMask.NameToLayer("FrontLaneBody");

        backLaneBodyLayer = LayerMask.NameToLayer("BackLaneBody");

        laneTransitionBodyLayer = LayerMask.NameToLayer("LaneTransitionBody");

        if(frontLaneBodyLayer == -1)
        {
            Debug.LogError("PlayerLaneController: Layer FrontLaneBody não encontrada.", this);
        }

        if(backLaneBodyLayer == -1)
        {
            Debug.LogError("PlayerLaneController: Layer BackLaneBody não encontrada.", this);
        }
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

        ApplyTransitionBodyLayer();

        Debug.Log($"[LANE] Transição iniciada. {currentLane} -> {targetLane}", this);

        return true;
    }

    public void CompleteTransition()
    {
        if(!isTransitioning)
            return;

        GameplayLane previousLane = currentLane;

        currentLane = targetLane;
        isTransitioning = false;

        ApplyLaneSettings(currentLane);

        OnLaneChanged?.Invoke(previousLane, currentLane);

        Debug.Log($"[LANE] Transição concluída: {previousLane} -> {currentLane}", this);
    }

    public void CancelTransition()
    {
        if(!isTransitioning)
            return;

        targetLane = currentLane;
        isTransitioning = false;

        ApplyLaneSettings(currentLane);

        Debug.Log($"[LANE] Transição cancelada. Lane atual: {currentLane}", this);
    }

    private void ApplyBodyLayer(GameplayLane lane)
    {
        if(bodyCollider == null)
        {
            Debug.Log("PlayerLaneController: BodyCollider não configurado.", this);

            return;
        }

        int targetLayer = lane == GameplayLane.Front ? frontLaneBodyLayer : backLaneBodyLayer;

        if(targetLayer == -1)
            return;

        bodyCollider.gameObject.layer = targetLayer;
    }

    private void ApplyGroundMask(GameplayLane lane)
    {
        if(playerMovement == null)
            return;

        LayerMask targetMask = lane == GameplayLane.Front ? frontGroundMask : backGroundMask;

        playerMovement.SetGroundLayerMask(targetMask);
    }

    private void ApplyLaneSettings(GameplayLane lane)
    {
        ApplyBodyLayer(lane);
        ApplyGroundMask(lane);
    }

    private void ApplyTransitionBodyLayer()
    {
        if(bodyCollider == null)
            return;

        if(laneTransitionBodyLayer == -1)
            return;

        bodyCollider.gameObject.layer = laneTransitionBodyLayer;
    }


}
