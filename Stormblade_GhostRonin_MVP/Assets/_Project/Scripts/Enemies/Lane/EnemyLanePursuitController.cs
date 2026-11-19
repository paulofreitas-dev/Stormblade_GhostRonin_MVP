using UnityEngine;

public class EnemyLanePursuitController : MonoBehaviour
{
    [Header("Pursuit Settings")]
    [SerializeField] private EnemyLanePursuitMode pursuitMode = EnemyLanePursuitMode.Disabled;

    [Header("Enemy References")]
    [SerializeField] private EnemyLaneController laneController;
    [SerializeField] private EnemyLaneTargetDetection targetDetection;
    [SerializeField] private EnemyLaneMovement laneMovement;
    [SerializeField] private Health enemyHealth;

    [Header("Scene References")]
    [SerializeField] private PlayerLaneController playerLaneController;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Runtime Debug")]
    [SerializeField] private bool hasEnteredCamera;
    private bool pendingPlayerLaneChange;

    private readonly Plane[] cameraPlanes = new Plane[6];
    private PlayerLaneController subscribedPlayer;

    private void Awake()
    {
        if (laneController == null)
            laneController = GetComponent<EnemyLaneController>();

        if (targetDetection == null)
            targetDetection = GetComponent<EnemyLaneTargetDetection>();

        if (laneMovement == null)
            laneMovement = GetComponent<EnemyLaneMovement>();

        if (enemyHealth == null)
            enemyHealth = GetComponent<Health>();

        if (visualRenderer == null)
            visualRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    private void OnEnable()
    {
        if (laneMovement != null)
            laneMovement.OnLaneMovementFinished += HandleMovementFinished;

        ResolveSceneReferences();
    }

    private void OnDisable()
    {
        if (laneMovement != null)
            laneMovement.OnLaneMovementFinished -= HandleMovementFinished;

        if (subscribedPlayer != null)
            subscribedPlayer.OnLaneChanged -= HandlePlayerLaneChanged;

        subscribedPlayer = null;

        pendingPlayerLaneChange = false;
    }

    private void Update()
    {
        if (pursuitMode == EnemyLanePursuitMode.Disabled)
            return;

        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        ResolveSceneReferences();

        if (!HasRequiredReferences())
            return;

        if (hasEnteredCamera)
            return;

        if (!IsInsideGameplayCamera())
            return;

        hasEnteredCamera = true;

        Debug.Log($"[ENEMY PURSUIT] {name} entrou na câmera. " + $"Modo: {pursuitMode}", this);

        // Primeira avaliação de lane
        TryMatchPlayerLane(); 
    }
    private void ResolveSceneReferences()
    {
        if (gameplayCamera == null)
            gameplayCamera = Camera.main;

        if (playerLaneController == null)
            playerLaneController = FindFirstObjectByType<PlayerLaneController>();

        if (subscribedPlayer == playerLaneController)
            return;

        if (subscribedPlayer != null)
            subscribedPlayer.OnLaneChanged -= HandlePlayerLaneChanged;

        subscribedPlayer = playerLaneController;

        if (subscribedPlayer != null)
            subscribedPlayer.OnLaneChanged += HandlePlayerLaneChanged;
    }

    private bool HasRequiredReferences()
    {
        return laneController != null &&
               targetDetection != null &&
               laneMovement != null &&
               playerLaneController != null &&
               gameplayCamera != null &&
               visualRenderer != null;
    }

    private bool IsInsideGameplayCamera()
    {
        if (!visualRenderer.enabled || !visualRenderer.gameObject.activeInHierarchy)
            return false;

        GeometryUtility.CalculateFrustumPlanes(gameplayCamera, cameraPlanes);

        return GeometryUtility.TestPlanesAABB(cameraPlanes, visualRenderer.bounds);
    }

    private void HandlePlayerLaneChanged(GameplayLane previousLane, GameplayLane newLane)
    {
        if (pursuitMode != EnemyLanePursuitMode.FollowPlayer)
            return;

        if (!hasEnteredCamera)
            return;

        if(enemyHealth != null && enemyHealth.IsDead)
            return;

        if(laneController == null || laneMovement == null)
            return;

        Debug.Log($"[ENEMY PURSUIT EVENT] Player concluiu " + $"{previousLane} -> {newLane}.",
        this);
        
        //Se o inimigo já estiver em transição,
        //registra a necessidade de reavaliar depois
        if(laneController.IsTransitioning || laneMovement.IsMovingBetweenLanes)
        {
            pendingPlayerLaneChange = true;
            return;
        }

        pendingPlayerLaneChange = false;

        TryMatchPlayerLane();
    }

    private void HandleMovementFinished()
    {
        if(!pendingPlayerLaneChange)
            return;
        
        pendingPlayerLaneChange = false;

        if (pursuitMode != EnemyLanePursuitMode.FollowPlayer)
            return;

        if (!hasEnteredCamera)
            return;

        // Processa uma mudança do Player recebida durante a transição anterior do inimigo
        TryMatchPlayerLane();
    }

    private bool TryMatchPlayerLane()
    {
        if (!HasRequiredReferences())
            return false;

        if (enemyHealth != null && enemyHealth.IsDead)
            return false;

        if (laneController.IsTransitioning || laneMovement.IsMovingBetweenLanes)
            return false;

        GameplayLane desiredLane = playerLaneController.CurrentLane;

        if (laneController.CurrentLane == desiredLane)
            return false;

        if (!targetDetection.TryFindTarget(desiredLane, out Vector2 destination))
            return false;

        if (!laneController.BeginTransition(desiredLane))
            return false;

        if (!laneMovement.StartMovement(destination))
        {
            laneController.CancelTransition();
            return false;
        }

        Debug.Log($"[ENEMY PURSUIT] {name} seguindo Player " + $"para {desiredLane}.",
        this);

        return true;
    }
}

