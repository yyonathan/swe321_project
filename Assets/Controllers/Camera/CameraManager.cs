using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float _orthographicSize;
    private Camera _cam;

    [SerializeField] private float _gracePeriod; // seconds given before camera starts moving
    [SerializeField] private float _initialAcceleration; // initial acceleration to get the camera up to speed
    [SerializeField] private float _actualAcceleration; // the actual rate at which the camera will accelerate for the rest of the gameplay loop
    [SerializeField] private float _endInitialAccelerationSpeed;
    [SerializeField] private float _maxSpeed; // i mean yea

    private float _timer;
    private float _currentSpeed;
    public float CurrentSpeed => _currentSpeed;
    public float EndInitialAccelerationSpeed => _endInitialAccelerationSpeed;

    private void Awake()
    {
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        _cam = GetComponent<Camera>();
        _cam.orthographic = true;
        _cam.orthographicSize = _orthographicSize;

        _orthographicSize = 5f;
        _timer = 0;
        _currentSpeed = 0;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_currentSpeed > _maxSpeed)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _maxSpeed * Time.deltaTime, transform.position.z);
        }
        else if (_currentSpeed > _endInitialAccelerationSpeed) // acceleration should slow down once it has initially caught up to speed

        {
            _currentSpeed += _actualAcceleration * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + _currentSpeed * Time.deltaTime, transform.position.z);
        }
        else if (_timer > _gracePeriod) // after grace period, camera should accelerate quickly to catch up
        {
            _currentSpeed += _initialAcceleration * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + _currentSpeed * Time.deltaTime, transform.position.z);
        }
    }
}
