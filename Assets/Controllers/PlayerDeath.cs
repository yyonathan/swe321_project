using UnityEngine;
using System;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _deathBuffer = 1f; // units below camera bottom before death triggers

    private bool _isDead = false;

    public static event Action OnPlayerDeath;

    private void Awake()
    {
        if (_cam == null) _cam = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        float cameraBottom = _cam.transform.position.y - _cam.orthographicSize;

        if (transform.position.y < cameraBottom - _deathBuffer)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        GetComponent<PlayerPhysics>().DisableMovement();
        OnPlayerDeath?.Invoke();
    }

    // call this when restarting to reset death state
    public void ResetDeath()
    {
        _isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDead) return;
        if (other.CompareTag("Spike"))
        {
            Die();
        }
    }
}

