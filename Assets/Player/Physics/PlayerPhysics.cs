using UnityEngine;
using System.Collections;

public class PlayerPhysics : MonoBehaviour
{
    // components
    public Rigidbody2D rb;

    // player data
    [SerializeField] private ControlsData _data;

    // contains live updated input data from whatever input device the player is using (touch, keyboard, whatever else we decide to implement which likely won't be much)
    [SerializeField] private PlayerInputState _inputState;

    // reference for camera manager for purpose of scaling jump force based on speed
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private bool _isPlayerTester;
    private float _scaledJumpForce;

    // core state
    private bool _isGrounded;
    private bool _jumpQueued;
    private bool _jumpCutQueued;

    private Vector2 _moveInput;

    [Header("raycasting")]
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private float _castDistance;
    [SerializeField] private LayerMask _surfaceLayer;

    // moving platform checks
    private MovingPlatform _currentPlatform;

    // cached values
    private float _currentMaxSpeed;
    private float _currentVelocityX;
    private float _velocityXSmoothing; // reference value that unity will use internally
    private float _jumpBufferCounter;
    private float _coyoteCounter;
    private float _accelDelta, _deccelDelta; // actual acceleration/decceleration force applied to player

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        _currentMaxSpeed = _data.BaseMaxSpeed;
        _scaledJumpForce = _data.BaseJumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        ReadAllInput();

    }

    void FixedUpdate()
    {
        ScaleJumpForce();
        UpdateGroundedState();
        ApplyPlatformVelocity();

        HandleJump();
        HandleJumpCut();
        ApplyFallGravity();
        ApplyTerminalVelocity();
        ApplyJumpApexBoost();
        Run();
    }

    private void ApplyPlatformVelocity()
    {
        if (_currentPlatform == null) return;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
    }

    private void UpdateGroundedState()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, _boxSize, 0, -transform.up, _castDistance, _surfaceLayer);

        if (hit.collider != null)
        {

            float surfaceTop = hit.collider.bounds.max.y;
            float playerBottom = GetComponent<Collider2D>().bounds.min.y;

            if (playerBottom >= surfaceTop - 0.05f)
            {
                _isGrounded = true;
                _coyoteCounter = _data.CoyoteTime;

                // check if standing on a moving platform
                _currentPlatform = hit.collider.GetComponent<MovingPlatform>();
            }
            else
            {
                _isGrounded = false;
                _coyoteCounter -= Time.fixedDeltaTime;

                _currentPlatform = null;
            }
        }
        else
        {
            _isGrounded = false;
            _coyoteCounter -= Time.fixedDeltaTime;

            _currentPlatform = null;
        }
    }

    // populating/updating necessary flags and other variables to be used for movement logic
    private void ReadAllInput()
    {
        _moveInput = _inputState.Move;

        if (_inputState.JumpPressed)
        {
            _jumpBufferCounter = _data.JumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0f && (_isGrounded || _coyoteCounter > 0f))
        {
            _jumpQueued = true;
        }

        if (_inputState.SupportsJumpCut && _inputState.JumpReleased)
        {
            _jumpCutQueued = true;
        }

        _inputState.ConsumeFrameInput();
    }

    private void ScaleJumpForce()
    {
        // > 1 because we dont want to descale the jump force
        if (_cameraManager.CurrentSpeed > _cameraManager.EndInitialAccelerationSpeed) 
        {
            _scaledJumpForce = _data.BaseJumpForce + (_cameraManager.CurrentSpeed - _cameraManager.EndInitialAccelerationSpeed) * 1.3f;

            // 8 * 1 = 8 + (1 - 1) * 1.5 = 8
            // 8 * 2 = 8 + (2 - 1) * 1.5 = 9.5
            // 8 * 3 = 8 + (3 - 1) * 1.5 = 11
        }
    }

    private void HandleJump()
    {
        if (!_jumpQueued)
        {
            return;
        }

        if (!(_isGrounded || _coyoteCounter > 0f))
        {
            return;
        }

        // if velocity is downward, cancel momentum and THEN apply the jump
        // (this lets you jump seamlessly while standing on a falling platform)
        // however, if you are moving up, momentum is conserved (idk if this is correct physics terminology but whatever)
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
        if (rb.linearVelocity.y > 0)
        {
            // divide a bit cuz the jump power u get from a moving platform is too much on its own
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y / 1.5f); 
        }

        rb.AddForce(Vector2.up * _scaledJumpForce, ForceMode2D.Impulse);

        _jumpQueued = false;
        _jumpCutQueued = false;
        _jumpBufferCounter = 0f;
        _coyoteCounter = 0f;
        _isGrounded = false;
    }

    private void HandleJumpCut()
    {
        if (!_jumpCutQueued)
        {
            return;
        }

        // only cut jump if still moving upward
        if (rb.linearVelocity.y > 0f)
        {
            rb.AddForce(Vector2.down * rb.linearVelocity.y * (1 - _data.JumpCutMultiplier), ForceMode2D.Impulse);
        }

        _jumpCutQueued = false;
    }

    private void ApplyFallGravity()
    {
        bool isFalling = !_isGrounded && rb.linearVelocity.y < 0f;

        if (isFalling)
        {
            rb.gravityScale = _data.RegularGravity * _data.FallingGravity;
        }
        else
        {
            rb.gravityScale = _data.RegularGravity;
        }
    }

    private void ApplyTerminalVelocity()
    {
        if (rb.linearVelocity.y < _data.TerminalVelocity)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, _data.TerminalVelocity);
        }
    }

    private void ApplyJumpApexBoost()
    {
        bool atJumpApex = !_isGrounded && Mathf.Abs(rb.linearVelocity.y) < _data.JumpApexRange;

        if (atJumpApex)
        {
            _currentMaxSpeed = _data.BaseMaxSpeed * _data.JumpApexMultiplier;
        }
        else
        {
            _currentMaxSpeed = _data.BaseMaxSpeed;
        }
    }

    private void Run()
    {
        float platformVelocityX = _currentPlatform != null ? _currentPlatform.DeltaPosition.x / Time.fixedDeltaTime : 0f;

        // desired speed after fully accelerating
        float targetSpeed = (_moveInput.x * _currentMaxSpeed) + platformVelocityX;

        // accounts for fixed deltatime and current max speed to do the accel time equation with proper values
        _accelDelta = _data.AccelAmount * Time.fixedDeltaTime * _currentMaxSpeed;
        _deccelDelta = _data.DeccelAmount * Time.fixedDeltaTime * _currentMaxSpeed;

        // choose acceleration vs deceleration time
        float accelTime = (Mathf.Abs(targetSpeed) > 0.01f) ? _accelDelta : _deccelDelta;

        // smoothly adjust velocity.x toward target
        _currentVelocityX = Mathf.SmoothDamp(rb.linearVelocity.x, targetSpeed, ref _velocityXSmoothing, accelTime, Mathf.Infinity, Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(_currentVelocityX, rb.linearVelocity.y);
    }

    // used purely for debugging purposes to view the hitbox of the ground detection
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * _castDistance, _boxSize);
    }
}