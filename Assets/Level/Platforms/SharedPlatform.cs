#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class SharedBehavior : MonoBehaviour
{
    // lockedx and lockedy keeps the platforms the same size, if enabled
    // i turned off lockx because i want variable length, but not variable thickness (height)
    // this will keep the game aesthetically pleasing, just to keep it from having random sizes everywhere. consistency
    [SerializeField] private float _lockedX = 1.2f;
    [SerializeField] private float _lockedY = 0.2f;
    [SerializeField] private bool _lockX = false;
    [SerializeField] private bool _lockY = true;

    private void Update()
    {
        if (transform.localScale.x != _lockedX && _lockX)
            transform.localScale = new Vector3(_lockedX, transform.localScale.y, transform.localScale.z);

        if (transform.localScale.y != _lockedY && _lockY)
            transform.localScale = new Vector3(transform.localScale.x, _lockedY, transform.localScale.z);
    }
}
#endif
