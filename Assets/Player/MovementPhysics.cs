using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // components
    public Rigidbody2D rb { get; private set; }

    // player data
    [SerializeField] private ControlsData _data;

    [SerializeField] private InputActionReference _move;
    [SerializeField] private InputActionReference _jump;

    /* bool checks: these are here for communication between update and fixed update*/
    // physics based bool checks
    private bool _isGrounded;
    private bool _isFalling;
    private bool _atTerminalVelocity;
    private bool _atJumpApex;

    // input based bool checks
    private bool _isJump;
    private bool _isJumpCut;
    private bool _isMidJump;

    private Vector2 _moveInput;

    // set all of these up in the inspector
    [Header("Raycasting")]
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private float _castDistance;
    [SerializeField] private LayerMask _groundLayer;

    // temporary or saved/cached variables
    private float _currentMaxSpeed;
    private float _currentVelocityX;
    private float _velocityXSmoothing; // reference value that unity will use internally for some reason
    private float _jumpBufferCounter;
    private float _coyoteCounter;
    private float _accelDelta, _deccelDelta; // actual acceleration/decceleration force applied to player

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // saves the base max speed so whenever the max speed itself is altered
        _currentMaxSpeed = _data.baseMaxSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

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
    void FixedUpdate()
    {
        // checks (assign boolean fields)
        IsGrounded(); // checks if you are standing on ground.
        IsFalling(); // checks if you are falling in the air (negative y velocity)
        AtTerminalVelocity();
        AtJumpApex();

        // apply forces
        FallFaster(); // if falling and not grounded, scale the gravity.
        ApplyTerminalVelocity(); // ensures you wont fall faster than the assigned terminal velocity.
        BoostJumpApex(); // increases max speed when at apex of jump
        Jump();
        JumpCut();

        Run(); 
    }

    // Update is called once per frame
    void Update()
    {
        // checks (assign boolean fields)
        ReadAllInput(); // enables and disables various input fields for methods in fixed update to read (things like _isJump and _isJumpCut)
    }

    /*
     * physics based checks (assign boolean fields)
    */
    // projects a raycast below the player to check if they are grounded
    // will be assigned in fixed updated since this is physics based
    private void IsGrounded()
    {
        if (Physics2D.BoxCast(transform.position, _boxSize, 0, -transform.up, _castDistance, _groundLayer))
        {
            _isGrounded = true;
            _isMidJump = false; // this is for jumping when falling off of a ledge
            _coyoteCounter = _data.coyoteTime; // resetting timer
        }
        else
        {
            _isGrounded = false;
            _coyoteCounter -= Time.fixedDeltaTime;
        }
    }

    // checks if player is falling by seeing if y velocity is negative and if the player is in the air.
    private void IsFalling()
    {
        if (rb.linearVelocity.y < 0 && !_isGrounded)
        {
            _isFalling = true;
        }
        else
        {
            _isFalling = false;
        }
    }

    // fixes terminal velocity boolean if the player is falling faster than terminal velocity
    private void AtTerminalVelocity()
    {
        _atTerminalVelocity = rb.linearVelocity.y < _data.terminalVelocity;
    }

    // checks player if they are at a jump apex (y velocity close to 0
    private void AtJumpApex()
    {
        _atJumpApex = !_isGrounded && Mathf.Abs(rb.linearVelocity.y) < _data.jumpApexRange;
    }

    /*
     * input based checks
     */
    private void ReadAllInput()
    {
        _moveInput = _move.action.ReadValue<Vector2>();

        if (_jump.action.WasPressedThisFrame())
        {
            _jumpBufferCounter = _data.jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0f && (_isGrounded || _coyoteCounter > 0f) && !_isMidJump)
        {
            _isJump = true;
        }

        if (_jump.action.WasReleasedThisFrame())
        {
            _isJumpCut = !_isFalling;
        }
    }


    /*
    apply forces
    */
    // does physics stuff to apply a jump to the player
    private void Jump()
    {
        if (_isJump)
        {
            // if  velocity is downward, cancel momentum and then apply the jump
            // (this lets you jump seamlessly while standing on a falling platform)
            // however, if you are moving up, momentum is conserved (idk if this is correct physics terminology but whatever)
            //if (rb.linearVelocity.y < 0) // logic not same as IsFalling(), so that is not used here
            //{
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            //}
            rb.AddForce(Vector2.up * _data.baseJumpForce, ForceMode2D.Impulse);
            _isJump = false;
            _isMidJump = true;
            _coyoteCounter = 0f;
            _jumpBufferCounter = 0f;
        }
    }

    // applies force to player when falling down and space bar is released
    // I NOW REALIZE AFTER IMPLEMENTING THIS that this won't exactly work with a swipe up
    // when handling mobile controls, this should 
    private void JumpCut()
    {
        if (_isJumpCut)
        {
            rb.AddForce(Vector2.down * rb.linearVelocity.y * (1 - _data.jumpCutMultiplier), ForceMode2D.Impulse);
            _isJumpCut = false;
        }
    }

    // while falling in the air, change gravity scale
    private void FallFaster()
    {
        if (_isFalling)
        {
            rb.gravityScale = _data.regularGravity * _data.fallingGravity;
        }
        else
        {
            rb.gravityScale = _data.regularGravity;
        }
    }

    // if at at terminal velocity, prevent player from going faster
    private void ApplyTerminalVelocity()
    {
        if (_atTerminalVelocity)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _data.terminalVelocity);
        }
    }

    // if at jump apex, apply a slight speed boost by slightly increasing max speed
    private void BoostJumpApex()
    {
        if (_atJumpApex)
        {
            _currentMaxSpeed = _data.baseMaxSpeed * _data.jumpApexMultiplier;
        }
        else
        {
            _currentMaxSpeed = _data.baseMaxSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * _castDistance, _boxSize);
    }

    private void Run()
    {
        // desired speed after fully accelerating
        float targetSpeed = _moveInput.x * _currentMaxSpeed;

        // accounts for fixed deltatime and current max speed to do the accel time equation with proper values
        _accelDelta = _data.accelAmount * Time.fixedDeltaTime * _currentMaxSpeed;
        _deccelDelta = _data.deccelAmount * Time.fixedDeltaTime * _currentMaxSpeed;

        // choose acceleration vs deceleration time
        float accelTime = (Mathf.Abs(targetSpeed) > 0.01f) ? _accelDelta : _deccelDelta;

        // smoothly adjust velocity.x toward target
        _currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetSpeed, ref _velocityXSmoothing, accelTime, Mathf.Infinity, Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(_currentVelocityX, rb.linearVelocity.y);
    }
}
