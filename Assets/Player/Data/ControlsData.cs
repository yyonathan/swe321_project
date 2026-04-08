using UnityEngine;

[CreateAssetMenu(fileName = "ControlsData", menuName = "Scriptable Objects/ControlsData")]
public class ControlsData : ScriptableObject
{
    // default v alues
    // base player movement stats
    [Header("Base Player Movement Stats")]
    [SerializeField] private float _baseMaxSpeed = 8; // the top horizontal movement speed
    [SerializeField] private float _baseJumpForce = 30; // the immediate velocity applied when jumping
    [SerializeField] private float _jumpApexMultiplier = 1.18F; // speed multiplier near jump apex
    [SerializeField] private float _jumpCutMultiplier = 0.3F; // reduces upward velocity when jump released early
    [SerializeField] private float _jumpApexRange = 10F; // vertical velocity range considered apex
    [SerializeField] private float _jumpBufferTime = 0.15F; // time window to buffer jump input
    [SerializeField] private float _coyoteTime = 0.1F; // tiny grace period usually to jump after walking off a ledge
    [SerializeField] private float _accelAmount = 0.2F, _deccelAmount = 0.15F; // acceleration and deceleration rates

    // player physics
    [Header("Player Physics")]
    [SerializeField] private float _regularGravity = 8; // default gravity scale applied to player
    [SerializeField] private float _fallingGravity = 2; // multiplier applied when falling
    [SerializeField] private float _terminalVelocity = -20; // maximum downward velocity cap


    // making each property readable but not writeable using syntactic lambda voodoo
    public float BaseMaxSpeed => _baseMaxSpeed;
    public float BaseJumpForce => _baseJumpForce; 
    public float JumpApexMultiplier => _jumpApexMultiplier;
    public float JumpCutMultiplier => _jumpCutMultiplier;
    public float JumpApexRange => _jumpApexRange;
    public float JumpBufferTime => _jumpBufferTime;
    public float CoyoteTime => _coyoteTime;
    public float AccelAmount => _accelAmount;
    public float DeccelAmount => _deccelAmount;
    public float RegularGravity => _regularGravity;
    public float FallingGravity => _fallingGravity;
    public float TerminalVelocity => _terminalVelocity;

}
