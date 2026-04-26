using UnityEngine;
using TMPro;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _deathCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _newBestText;
    [SerializeField] private TitleScreen _titleScreen;

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDeath += HandleDeath;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        _deathCanvas.SetActive(true);

        float score = ScoreManager.Instance.CurrentScore;
        _scoreText.text = $"{score:F1}s";

        _newBestText.text = ScoreManager.Instance.IsNewBest ? "New Best!" : "Score";
    }

    public void OnMenuButtonPressed()
    {
        _deathCanvas.SetActive(false);
        _titleScreen.ReturnToMenu();
    }
}
