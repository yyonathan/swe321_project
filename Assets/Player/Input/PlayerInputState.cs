using UnityEngine;

public class PlayerInputState : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool SupportsJumpCut { get; private set; }

    public void SetMove(Vector2 move)
    {
        Move = move;
    }

    public void SetJumpPressed(bool value)
    {
        JumpPressed = value;
    }

    public void SetJumpReleased(bool value)
    {
        JumpReleased = value;
    }

    public void SetSupportsJumpCut(bool value)
    {
        SupportsJumpCut = value;
    }

    // call this once per frame after PlayerController reads the values
    public void ConsumeFrameInput()
    {
        JumpPressed = false;
        JumpReleased = false;
    }
}