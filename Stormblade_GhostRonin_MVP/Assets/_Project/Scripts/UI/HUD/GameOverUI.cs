using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLifePoints playerLifePoints;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if(gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if(playerLifePoints != null)
            playerLifePoints.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        if(playerLifePoints != null)
            playerLifePoints.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        if(gameOverPanel == null)
            return;

        gameOverPanel.SetActive(true);
    }
}
