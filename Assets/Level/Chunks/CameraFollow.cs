using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followSpeed = 3f;
    [SerializeField] private float _verticalOffset = 0f;

    private void Awake()
    {
        transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        float targetY = Mathf.Lerp(transform.position.y, _target.position.y + _verticalOffset, Time.deltaTime * _followSpeed);
        transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
    }
}

