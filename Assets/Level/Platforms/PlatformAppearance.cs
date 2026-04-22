using UnityEngine;

public class PlatformAppearance : MonoBehaviour
{
    private static readonly int SeedProperty = Shader.PropertyToID("_Seed");

    private void Awake()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.GetPropertyBlock(block);
        block.SetFloat(SeedProperty, Random.Range(0f, 1000f));
        block.SetFloat("_AspectRatio", transform.lossyScale.x / transform.lossyScale.y);
        sr.SetPropertyBlock(block);
    }
}