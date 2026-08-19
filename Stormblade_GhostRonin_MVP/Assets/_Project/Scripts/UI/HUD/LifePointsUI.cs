using UnityEngine;
using TMPro;

public class LifePointsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLifePoints playerLifePoints;
    [SerializeField] private TMP_Text lifePointsText;

    private void OnEnable()
    {
        if(playerLifePoints != null)
            playerLifePoints.OnLifePointsChanged += UpdateLifePoints;
    }

    private void Start()
    {
        RefreshLifePoints();
    }

    private void OnDisable()
    {
        if(playerLifePoints != null)
            playerLifePoints.OnLifePointsChanged -= UpdateLifePoints;
    }

    private void RefreshLifePoints()
    {
        if(playerLifePoints == null)
            return;

        UpdateLifePoints(playerLifePoints.CurrentLifePoints);
    }

    private void UpdateLifePoints(int currentLifePoints)
    {
        if(lifePointsText == null)
            return;

        lifePointsText.text = $"x{currentLifePoints}";
    }
}
