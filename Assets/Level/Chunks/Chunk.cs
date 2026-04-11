using UnityEngine;

[ExecuteAlways]
public class SharedPlatformBehavior : MonoBehaviour
{
    [SerializeField] private float _chunkWidth = 4.8f;
    [SerializeField] private float _verticalPadding = 0.5f;
    [SerializeField] private Difficulty difficulty = Difficulty.Starting;

    private float _upperTransitionBorder;
    private float _lowerTransitionBorder;

    // getters
    public float UpperTransitionBorder => _upperTransitionBorder;
    public float LowerTransitionBorder => _lowerTransitionBorder;
    public Difficulty ChunkDifficulty => difficulty;


    // displays the chunk borders in the editor.
    // the only relevant information is the vertical borders, as those will be the transitions.
    // the width is just some arbitrarily value chosen close to what the width of the screen might be
    private void Awake()
    {
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Collider2D col = transform.GetChild(i).GetComponent<Collider2D>();
            if (col == null) continue;

            if (col.bounds.min.y < minY)
            {
                minY = col.bounds.min.y;
                _lowerTransitionBorder = minY - _verticalPadding;
            }

            if (col.bounds.max.y > maxY)
            {
                maxY = col.bounds.max.y;
                _upperTransitionBorder = maxY + _verticalPadding;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (transform.childCount == 0) return;

        else if (difficulty == Difficulty.Starting)
        {
            Gizmos.color = Color.deepSkyBlue;
        }
        else if (difficulty == Difficulty.Easy)
        {
            Gizmos.color = Color.limeGreen;
        }
        else if (difficulty == Difficulty.Medium)
        {
            Gizmos.color = Color.yellow;
        }
        else if (difficulty == Difficulty.Hard)
        {
            Gizmos.color = Color.softRed;
        }

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < transform.childCount; i++)
        {
            Collider2D col = transform.GetChild(i).GetComponent<Collider2D>();
            if (col == null) continue;

            if (col.bounds.min.y < minY)
            {
                minY = col.bounds.min.y;
                _lowerTransitionBorder = minY - _verticalPadding;
            }

            if (col.bounds.max.y > maxY)
            {
                maxY = col.bounds.max.y;
                _upperTransitionBorder = maxY + _verticalPadding;
            }
        }

        float centerY = (minY + maxY) / 2f;
        float height = maxY - minY;

        Gizmos.DrawWireCube(new Vector3(transform.position.x, centerY, 0f), new Vector3(_chunkWidth, height + _verticalPadding * 2, 0f));
    }
}