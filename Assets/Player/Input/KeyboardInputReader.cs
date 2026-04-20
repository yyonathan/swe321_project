using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputReader : MonoBehaviour
{
    [SerializeField] private PlayerInputState _inputState;
    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _jump;
    private bool _wasMovingLastFrame;

    private void OnEnable()
    {
        _move.action.Enable();
        _jump.action.Enable();
    }

    private void OnDisable()
    {
        _move.action.Disable();
        _jump.action.Disable();
    }

    private void Update()
    {
        Vector2 move = _move.action.ReadValue<Vector2>();
        if (move != Vector2.zero || _wasMovingLastFrame)
            _inputState.SetMove(move);
        _wasMovingLastFrame = move != Vector2.zero;

        _inputState.SetSupportsJumpCut(true);

        if (_jump.action.WasPressedThisFrame())
        {
            _inputState.SetJumpPressed(true);
        }

        if (_jump.action.WasReleasedThisFrame())
        {
            _inputState.SetJumpReleased(true);
        }
    }
}