using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlatformShadow : MonoBehaviour
{
    [Header("shadow settings")]
    [SerializeField] private float _shadowLength = 3f;
    [SerializeField] private float _shadowOffset = 0f;
    [SerializeField] private Material _shadowMaterial;

    private SpriteRenderer _spriteRenderer;
    private GameObject _shadowObject;
    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;
    private MaterialPropertyBlock _propertyBlock;
    private static readonly int AlphaProperty = Shader.PropertyToID("_Alpha");

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();

        CreateShadowMesh();
    }

    private void CreateShadowMesh()
    {
        _shadowObject = new GameObject("Shadow");
        _shadowObject.transform.SetParent(transform);
        _shadowObject.transform.localPosition = Vector3.zero;
        _shadowObject.transform.localRotation = Quaternion.identity;
        _shadowObject.transform.localScale = Vector3.one;

        _meshRenderer = _shadowObject.AddComponent<MeshRenderer>();

        // load shadow material from resources
        _meshRenderer.material = _shadowMaterial;
        _meshRenderer.sortingLayerName = _spriteRenderer.sortingLayerName;
        _meshRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;

        _meshFilter = _shadowObject.AddComponent<MeshFilter>();
        _meshFilter.mesh = new Mesh();
    }

    private void LateUpdate()
    {
        if (_shadowObject == null) return;
        UpdateShadow();
    }

    private void UpdateShadow()
    {
        // get platform size in local space
        Bounds localBounds = _spriteRenderer.sprite.bounds;
        float halfWidth = localBounds.extents.x * transform.localScale.x;
        float halfHeight = localBounds.extents.y * transform.localScale.y;
        float bottomY = -halfHeight - _shadowOffset;

        // four vertices in local space
        Vector3 topLeft = new Vector3(-halfWidth, bottomY, 0);
        Vector3 topRight = new Vector3(halfWidth, bottomY, 0);
        Vector3 botLeft = new Vector3(-halfWidth, bottomY - _shadowLength, 0);
        Vector3 botRight = new Vector3(halfWidth, bottomY - _shadowLength, 0);

        Mesh mesh = _meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] { topLeft, topRight, botLeft, botRight };
        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.triangles = new int[] { 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateNormals();

        // respect platform alpha for disappearing platforms
        _meshRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(AlphaProperty, _spriteRenderer.color.a);
        _meshRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void OnDestroy()
    {
        if (_shadowObject != null)
            Destroy(_shadowObject);
    }
}
