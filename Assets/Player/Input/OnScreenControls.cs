using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class OnScreenControls : MonoBehaviour
{
    [SerializeField] private PlayerInputState _inputState;

    [Header("button zones")]
    [SerializeField] private RectTransform _leftButton;
    [SerializeField] private RectTransform _rightButton;
    [SerializeField] private RectTransform _jumpButton;

    // tracks which zone each finger was in last frame so we only fire jump once per entry
    private Dictionary<int, Zone> _previousZones = new();
    private bool _hadDirectionalTouchLastFrame;

    private enum Zone { None, Left, Right, Jump }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        bool hasLeft = false;
        bool hasRight = false;
        bool jumpThisFrame = false;

        var activeTouches = Touch.activeTouches;

        // clean up fingers that are no longer active
        List<int> toRemove = null;
        foreach (var kvp in _previousZones)
        {
            bool found = false;
            foreach (var touch in activeTouches)
                if (touch.finger.index == kvp.Key) { found = true; break; }

            if (!found)
            {
                if (kvp.Value == Zone.Jump)
                    _inputState.SetJumpReleased(true);
                toRemove ??= new List<int>();
                toRemove.Add(kvp.Key);
            }
        }
        if (toRemove != null)
            foreach (int id in toRemove)
                _previousZones.Remove(id);

        foreach (var touch in activeTouches)
        {
            int id = touch.finger.index;
            Zone current = GetZone(touch.screenPosition);

            if (!_previousZones.TryGetValue(id, out Zone previous))
                previous = Zone.None;

            // fire jump only on entry into jump zone
            if (current == Zone.Jump && previous != Zone.Jump)
                jumpThisFrame = true;

            _previousZones[id] = current;

            if (current == Zone.Left) hasLeft = true;
            else if (current == Zone.Right) hasRight = true;
        }

        float moveX = 0f;
        if (hasLeft && !hasRight) moveX = -1f;
        else if (hasRight && !hasLeft) moveX = 1f;

        bool wasDirectional = _hadDirectionalTouchLastFrame;
        _hadDirectionalTouchLastFrame = hasLeft || hasRight;

        if (hasLeft || hasRight)
            _inputState.SetMove(new Vector2(moveX, 0f));
        else if (wasDirectional && !hasLeft && !hasRight)
            _inputState.SetMove(Vector2.zero);

        _inputState.SetSupportsJumpCut(true);

        if (jumpThisFrame)
            _inputState.SetJumpPressed(true);
    }

    private Zone GetZone(Vector2 screenPosition)
    {
        if (RectContains(_leftButton, screenPosition)) return Zone.Left;
        if (RectContains(_rightButton, screenPosition)) return Zone.Right;
        if (RectContains(_jumpButton, screenPosition)) return Zone.Jump;
        return Zone.None;
    }

    // converts screen position to local rect space and checks if it falls within the button bounds
    private bool RectContains(RectTransform rect, Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, null);
    }
}
