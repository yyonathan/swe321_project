using UnityEngine;
using System.Collections;

public class DisappearingPf : MonoBehaviour
{
    [Header("timing")]
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private float _disableDuration = 0.1f;

    [Header("detection")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _detectHeight = 0.1f;
    [SerializeField] private float _detectYOffset = 0.05f;

    private SpriteRenderer _sr;
    private BoxCollider2D _col;
    private float _disableTimer = 0f;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        float currentAlpha = GetAlpha();
        float fadeSpeed = 1f / _fadeDuration;

        if (!_col.enabled)
        {
            _disableTimer -= Time.fixedDeltaTime;
            if (_disableTimer <= 0f)
            {
                _col.enabled = true;
                _disableTimer = 0f;
            }
            return;
        }

        if (IsPlayerStandingOnTop())
        {
            float newAlpha = Mathf.MoveTowards(currentAlpha, 0f, fadeSpeed * Time.fixedDeltaTime);
            SetAlpha(newAlpha);

            if (newAlpha == 0f)
            {
                _col.enabled = false;
                _disableTimer = _disableDuration;
            }
        }
        else
        {
            SetAlpha(Mathf.MoveTowards(currentAlpha, 1f, fadeSpeed * Time.fixedDeltaTime));
        }
    }

    private bool IsPlayerStandingOnTop()
    {
        Bounds b = _col.bounds;
        Vector2 center = new(b.center.x, b.max.y + _detectHeight * 0.5f + _detectYOffset);
        Vector2 size = new(b.size.x, _detectHeight);
        return Physics2D.OverlapBox(center, size, 0f, _playerLayer) != null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = _sr.color;
        c.a = alpha;
        _sr.color = c;
    }

    private float GetAlpha() => _sr.color.a;

    private void OnDrawGizmosSelected()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Bounds b = col.bounds;
        Vector2 center = new(b.center.x, b.max.y + _detectHeight * 0.5f + _detectYOffset);
        Vector2 size = new(b.size.x, _detectHeight);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
