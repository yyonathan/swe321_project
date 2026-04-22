using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject titleCanvas;
    [SerializeField] GameObject player;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject HUD;
    Vector3 startPos;

    void Start()
    {
        startPos = player.transform.position;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        HUD.SetActive(false);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        HUD.SetActive(true);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void Restart()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StartGame();


    }

    public void Home()
    {
        Time.timeScale = 0f;
        HUD.SetActive(false);
        pauseMenu.SetActive(false);
        player.transform.position = startPos;
        titleCanvas.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        player.SetActive(false);
        pauseButton.SetActive(true);
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