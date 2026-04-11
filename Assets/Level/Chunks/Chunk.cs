using UnityEngine;

[ExecuteAlways]
public class Chunk : MonoBehaviour
{
    [SerializeField] private float _chunkWidth = 4.8f;
    [SerializeField] private float _verticalPadding = 0.5f;
    [SerializeField] private Difficulty _difficulty = Difficulty.Starting;

    private float _lowerBorderOffset;
    private float _upperBorderOffset;

    public float LowerTransitionBorder => transform.position.y + _lowerBorderOffset;
    public float UpperTransitionBorder => transform.position.y + _upperBorderOffset;
    public Difficulty ChunkDifficulty => _difficulty;

    private void Awake()
    {
        if (transform.childCount == 0) return;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Collider2D col = transform.GetChild(i).GetComponent<Collider2D>();
            if (col == null) continue;

            if (col.bounds.min.y < minY) minY = col.bounds.min.y;
            if (col.bounds.max.y > maxY) maxY = col.bounds.max.y;
        }

        _lowerBorderOffset = (minY - _verticalPadding) - transform.position.y;
        _upperBorderOffset = (maxY + _verticalPadding) - transform.position.y;
    }

    private void OnDrawGizmos()
    {
        if (transform.childCount == 0) return;

        if (_difficulty == Difficulty.Starting) Gizmos.color = Color.cyan;
        else if (_difficulty == Difficulty.Easy) Gizmos.color = Color.green;
        else if (_difficulty == Difficulty.Medium) Gizmos.color = Color.yellow;
        else if (_difficulty == Difficulty.Hard) Gizmos.color = Color.red;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Collider2D col = transform.GetChild(i).GetComponent<Collider2D>();
            if (col == null) continue;

            if (col.bounds.min.y < minY) minY = col.bounds.min.y;
            if (col.bounds.max.y > maxY) maxY = col.bounds.max.y;
        }

        float centerY = (minY + maxY) / 2f;
        float height = maxY - minY;

        Gizmos.DrawWireCube(
            new Vector3(transform.position.x, centerY, 0f),
            new Vector3(_chunkWidth, height + _verticalPadding * 2, 0f)
        );
    }
}
