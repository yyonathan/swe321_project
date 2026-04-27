using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleCanvas;
    public GameObject player;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private PlayerDeath _playerDeath;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private ChunkManager _chunkManager;
    [SerializeField] private MusicManager _musicManager;

    private Vector3 _playerStartPos;

    void Start()
    {
        _playerStartPos = player.transform.position;

        Time.timeScale = 0f;
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

        Time.timeScale = 1f;
        _musicManager.StartMusic();
        ScoreManager.Instance.StartRun();
    }

    public void ReturnToMenu()
    {
        _cameraManager.Reset();
        _chunkManager.Reset();

        player.transform.position = _playerStartPos;
        _playerDeath.ResetDeath();

        HUD.SetActive(false);
        pauseButton.SetActive(false);
        titleCanvas.SetActive(true);

        Time.timeScale = 0f;
        _musicManager.StopMusic();
    }
}