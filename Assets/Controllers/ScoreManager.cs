using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private const string PersonalBestKey = "PersonalBest";

    private float _currentScore = 0f;
    private float _personalBest = 0f;
    private bool _isRunning = false;

    public float CurrentScore => _currentScore;
    public float PersonalBest => _personalBest;
    public bool IsNewBest { get; private set; }

    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _personalBest = PlayerPrefs.GetFloat(PersonalBestKey, 0f);
    }

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDeath += HandleDeath;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDeath -= HandleDeath;
    }

    private void Update()
    {
        if (_isRunning)
        {
            _currentScore += Time.deltaTime;
        }
    }

    public void StartRun()
    {
        _currentScore = 0f;
        IsNewBest = false;
        _isRunning = true;
    }

    private void HandleDeath()
    {
        _isRunning = false;

        if (_currentScore > _personalBest)
        {
            _personalBest = _currentScore;
            IsNewBest = true;
            PlayerPrefs.SetFloat(PersonalBestKey, _personalBest);
            PlayerPrefs.Save();
        }
        else
        {
            IsNewBest = false;
        }
    }
}
