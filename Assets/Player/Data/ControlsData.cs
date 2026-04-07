using UnityEngine;

[CreateAssetMenu(fileName = "ControlsData", menuName = "Scriptable Objects/ControlsData")]
public class ControlsData : ScriptableObject
{
    // base player movement stats
    [Header("Base Player Movement Stats")]
    public float baseMaxSpeed; // the top horizontal movement speed
    public float baseJumpForce; // the immediate velocity applied when jumping
    public float jumpApexMultiplier; // speed multiplier near jump apex
    public float jumpCutMultiplier; // reduces upward velocity when jump released early
    public float jumpApexRange; // vertical velocity range considered apex
    public float jumpBufferTime; // time window to buffer jump input
    public float coyoteTime; // tiny grace period usually to jump after walking off a ledge
    public float accelAmount, deccelAmount; // acceleration and deceleration rates

    // player physics
    [Header("Player Physics")]
    public float regularGravity; // default gravity scale applied to player
    public float fallingGravity; // multiplier applied when falling
    public float terminalVelocity; // maximum downward velocity cap
}
