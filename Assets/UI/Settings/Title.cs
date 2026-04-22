using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleCanvas;
    public GameObject player;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject pauseButton;

    void Start()
    {
        Time.timeScale = 0f; // 🔥 freeze game at start
        HUD.SetActive(false);
        titleCanvas.SetActive(true);
        player.SetActive(false);
        pauseButton.SetActive(false);
    }

    public void StartGame()
    {
        titleCanvas.SetActive(false);
        HUD.SetActive(true);
        player.SetActive(true);
        pauseButton.SetActive(true);

        Time.timeScale = 1f; // start game
    }
}