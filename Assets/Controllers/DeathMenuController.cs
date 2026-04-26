using UnityEngine;
using TMPro;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _deathCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _newBestText;

    [SerializeField] private TitleScreen _titleScreen;
    [SerializeField] private PlayerDeath _playerDeath;
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

        _newBestText.text = ScoreManager.Instance.IsNewBest ? "New Best!" : "Score Achieved";
    }

    // hooked up to MenuButton's OnClick in the inspector
    public void OnMenuButtonPressed()
    {
        _deathCanvas.SetActive(false);
        _playerDeath.ResetDeath();
        ScoreManager.Instance.StartRun();
        _titleScreen.ReturnToMenu();
    }
}
