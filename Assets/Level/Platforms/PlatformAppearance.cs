using UnityEngine;

public class PlatformAppearance : MonoBehaviour
{
    private static readonly int SeedProperty = Shader.PropertyToID("_Seed");
    private static readonly int ColorBaseProperty = Shader.PropertyToID("_ColorBase");
    private static readonly int ColorLayerProperty = Shader.PropertyToID("_ColorLayer");
    private static readonly int AspectRatioProperty = Shader.PropertyToID("_AspectRatio");

    [SerializeField] private float _darkOffset = 0.15f;  // how much darker the base color is
    [SerializeField] private float _brightOffset = 0.1f; // how much brighter the layer color is

    private void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        sr.GetPropertyBlock(block);

        // derive base and layer colors from the sprite renderer's existing color
        Color spriteColor = sr.color;
        Color.RGBToHSV(spriteColor, out float h, out float s, out float v);

        Color baseColor = Color.HSVToRGB(h, s, Mathf.Clamp01(v - _darkOffset));
        Color layerColor = Color.HSVToRGB(h, Mathf.Clamp01(s - 0.1f), Mathf.Clamp01(v + _brightOffset));

        baseColor.a = spriteColor.a;
        layerColor.a = spriteColor.a;

        block.SetColor(ColorBaseProperty, baseColor);
        block.SetColor(ColorLayerProperty, layerColor);
        block.SetFloat(SeedProperty, Random.Range(0f, 1000f));
        block.SetFloat(AspectRatioProperty, transform.lossyScale.x / transform.lossyScale.y);

        sr.SetPropertyBlock(block);
    }
}
