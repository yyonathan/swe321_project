using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject TitleCanvas;
    public GameObject Player;

    void Start()
    {
        Player.SetActive(false);
    }

    public void StartGame()
    {
        TitleCanvas.SetActive(false);
        Player.SetActive(true);
    }
}