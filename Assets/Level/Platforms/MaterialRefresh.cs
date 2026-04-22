using UnityEngine;

public class MaterialRefresh : MonoBehaviour
{
    private void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.material = sr.material;
    }
}
