using UnityEngine;

public class PlatformShaderController : MonoBehaviour
{
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private Material _platformMaterial;
    [SerializeField] private float _baseSpeed = 1.2f;
    [SerializeField] private float _speedMultiplier = 0.5f;

    private void Update()
    {
        float speed = _baseSpeed + _cameraManager.CurrentSpeed * _speedMultiplier;
        _platformMaterial.SetFloat("_Speed", speed);
    }
}
