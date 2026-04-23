using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private float _minParallaxFactor = 0.1f;
    [SerializeField] private float _maxParallaxFactor = 0.4f;

    private ParticleSystem _ps;
    private ParticleSystem.Particle[] _particles;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _particles = new ParticleSystem.Particle[_ps.main.maxParticles];
    }

    private void Update()
    {
        int count = _ps.GetParticles(_particles);

        for (int i = 0; i < count; i++)
        {
            _particles[i].velocity = Vector3.up * _cameraManager.CurrentSpeed * Random.Range(_minParallaxFactor, _maxParallaxFactor);
        }

        _ps.SetParticles(_particles, count);
    }
}
