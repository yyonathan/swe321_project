using UnityEngine;
using TMPro;

public class ScoreTimer : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        PlayerDeath.OnPlayerDeath += Hide;
    }

    private void OnDisable()
    {
        PlayerDeath.OnPlayerDeath -= Hide;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _text.text = $"{ScoreManager.Instance.CurrentScore:F1}s";
    }
}
