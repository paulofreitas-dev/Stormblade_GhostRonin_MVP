using UnityEngine;
using UnityEngine.Rendering;

public class PlayerLaneVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualDepthRoot;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private PlayerLaneController laneController;
    [SerializeField] private PlayerLaneMovement laneMovement;

    [Header("Depth Scale")]
    [SerializeField] private float frontScale = 1f;
    [SerializeField] private float backScale = 0.8f;

    [Header("Sorting")]
    [SerializeField] private int frontSortingOrder = 10;
    [SerializeField] private int backSortingOrder = 5;

    private void Awake()
    {
        if(sortingGroup == null)
            sortingGroup = GetComponent<SortingGroup>();
    }

    private void LateUpdate()
    {
        UpdateDepthScale();
        UpdateSorting();
    }

    private void UpdateSorting()
    {
        if(sortingGroup == null || laneController == null)
            return;
        
        sortingGroup.sortingOrder = laneController.CurrentLane == GameplayLane.Front ? frontSortingOrder : backSortingOrder;
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
}
