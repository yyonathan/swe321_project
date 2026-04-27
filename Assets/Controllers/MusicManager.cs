using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Stop();
    }

    public void StartMusic()
    {
        _audioSource.Stop();
        _audioSource.Play(); // always restarts from beginning
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}