using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("anchors")]
    [SerializeField] private Transform _anchorA;
    [SerializeField] private Transform _anchorB;

    [Header("movement")]
    [SerializeField] private float _travelTime = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _startProgress = 0f;

    private float _journeyTimer = 0f;
    private bool _movingToB = true;

    // used for keeping player on platform
    private Vector2 _previousPosition;
    public Vector2 DeltaPosition { get; private set; }

    private Vector2 _posA;
    private Vector2 _posB;

    // used for reading for the chunk border (if an anchor is the highest object in a chunk, adjust the border to the anchor)
    public float GetMinY() => _anchorA != null && _anchorB != null ? Mathf.Min(_anchorA.position.y, _anchorB.position.y) : transform.position.y;
    public float GetMaxY() => _anchorA != null && _anchorB != null ? Mathf.Max(_anchorA.position.y, _anchorB.position.y) : transform.position.y;

    // separated for the gizmo to get it to work
    public float GetMinYLocal() => GetMinY();
    public float GetMaxYLocal() => GetMaxY();

    private Rigidbody2D _rb;

    private bool _initialized = false;

    private void Awake()
    {
        _anchorA.GetComponent<SpriteRenderer>().enabled = false;
        _anchorB.GetComponent<SpriteRenderer>().enabled = false;

        _rb = GetComponent<Rigidbody2D>();

        _journeyTimer = _startProgress * _travelTime;
    }

    private void FixedUpdate()
    {
        // cache anchor world points to use instead of their local positions
        if (!_initialized)
        {
            _posA = _anchorA.position;
            _posB = _anchorB.position;
            _initialized = true;
        }

        if (_anchorA == null || _anchorB == null) return;

        _journeyTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(_journeyTimer / _travelTime);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector2 targetPos = _movingToB ? Vector2.Lerp(_posA, _posB, smoothT) : Vector2.Lerp(_posB, _posA, smoothT);

        // capture how much we're about to move so the player can inherit it
        DeltaPosition = targetPos - (Vector2)transform.position;
        _rb.MovePosition(targetPos);

        if (t >= 1f)
        {
            _movingToB = !_movingToB;
            _journeyTimer = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        if (_anchorA == null || _anchorB == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_anchorA.position, 0.15f);
        Gizmos.DrawWireSphere(_anchorB.position, 0.15f);
        Gizmos.DrawLine(_anchorA.position, _anchorB.position);
    }
}