using UnityEngine;
using UnityEngine.Rendering;

public class EnemyLaneVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualDepthRoot;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private EnemyLaneController laneController;

    [Header("Depth Scale")]
    [SerializeField] private float frontScale = 1f;
    [SerializeField] private float backScale = 0.8f;

    [Header("Sorting")]
    [SerializeField] private int frontSortingOrder = 10;
    [SerializeField] private int backSortingOrder = 5;

    private void Awake()
    {
        if(laneController == null)
            laneController = GetComponent<EnemyLaneController>();

        if(sortingGroup == null && visualDepthRoot != null)
            sortingGroup = visualDepthRoot.GetComponent<SortingGroup>();
    }

    private void OnEnabled()
    {
        if(laneController != null)
            laneController.OnLaneChanged += HandleLaneChanged;
    }

    private void Start()
    {
        ApplyLaneVisual();
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

    private void ApplyLaneVisual()
    {
        if(laneController == null)
            return;

        GameplayLane lane = laneController.CurrentLane;

        float targetScale = lane == GameplayLane.Front ? frontScale : backScale;

        if(visualDepthRoot != null)
            visualDepthRoot.localScale = new Vector3(targetScale, targetScale, 1f);

        if(sortingGroup != null)
            sortingGroup.sortingOrder = lane == GameplayLane.Front ? frontSortingOrder : backSortingOrder;
    }
}
