using UnityEngine;
using UnityEngine.Rendering;

public class EnemyLaneVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualDepthRoot;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private EnemyLaneController laneController;
    [SerializeField] private EnemyLaneMovement laneMovement;

    [Header("Depth Scale")]
    [SerializeField] private float frontScale = 1f;
    [SerializeField] private float backScale = 0.8f;

    [Header("Sorting")]
    [SerializeField] private int frontSortingOrder = 15;
    [SerializeField] private int backSortingOrder = 14;

    private void Awake()
    {
        if(visualDepthRoot == null)
            visualDepthRoot = transform;

        if(laneController == null)
            laneController = GetComponentInParent<EnemyLaneController>();

        if(laneMovement == null)
            laneMovement = GetComponentInParent<EnemyLaneMovement>();

        if(sortingGroup == null && visualDepthRoot != null)
            sortingGroup = visualDepthRoot.GetComponent<SortingGroup>();
    }

    private void OnEnable()
    {
        if(laneController != null)
            laneController.OnLaneChanged += HandleLaneChanged;
    }

    private void Start()
    {
        ApplyLaneVisual();
    }

    private void LateUpdate()
    {
        UpdateDepthScale();
    }

    private void OnDisable()
    {
        if(laneController != null)
            laneController.OnLaneChanged -= HandleLaneChanged;
    }

    private void HandleLaneChanged(GameplayLane previousLane, GameplayLane newLane)
    {
        ApplyLaneVisual();
    }

    private void UpdateDepthScale()
    {
        if(visualDepthRoot == null || laneController == null)
            return;

        float targetScale;

        if(laneMovement != null && laneMovement.IsMovingBetweenLanes)
        {
            float startScale = GetScaleForLane(laneController.CurrentLane);

            float endScale = GetScaleForLane(laneController.TargetLane);

            float t = Mathf.SmoothStep(0f, 1f, laneMovement.TransitionProgress);

            targetScale = Mathf.Lerp(startScale, endScale, t);
        }

        else
        {
            targetScale = GetScaleForLane(laneController.CurrentLane);
        }

        visualDepthRoot.localScale = new Vector3(targetScale, targetScale, 1f);
    }

    private float GetScaleForLane(GameplayLane lane)
    {
        return lane == GameplayLane.Front ? frontScale : backScale;
    }

    private void ApplyLaneVisual()
    {
        if(laneController == null)
            return;

        GameplayLane lane = laneController.CurrentLane;

        float targetScale = GetScaleForLane(lane);

        if(visualDepthRoot != null)
            visualDepthRoot.localScale = new Vector3(targetScale, targetScale, 1f);

        if(sortingGroup != null)
            sortingGroup.sortingOrder = lane == GameplayLane.Front ? frontSortingOrder : backSortingOrder;
    }
}
