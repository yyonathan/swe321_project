using UnityEngine;
using TMPro;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _deathCanvas;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _newBestText;
    [SerializeField] private TitleScreen _titleScreen;
    [SerializeField] private TMP_InputField _nameInputField;

    [SerializeField] private Leaderboard _leaderboard;

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
        _scoreText.text = $"{score:F2}";

        _newBestText.text = ScoreManager.Instance.IsNewBest ? "New Best!" : "Score";

        // pre fill with saved name if one exists
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        _nameInputField.text = savedName;
    }

    // hooked up to MenuButton's OnClick in the inspector
    public void OnMenuButtonPressed()
    {
        string name = _nameInputField.text.Trim();

        if (!string.IsNullOrEmpty(name))
        {
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();
            _leaderboard.Submit(name);
        }

        _deathCanvas.SetActive(false);
        _titleScreen.ReturnToMenu();
    }
}