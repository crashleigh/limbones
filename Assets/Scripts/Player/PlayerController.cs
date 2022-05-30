using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayerCOntroller
{
    [SerializeField] private bool allowDash, allowDoubleJump, allowCrouch;

    public FrameInput Input { get; private set; }
    public Vector2 RawMovement { get; private set; }
    public bool Grounded => isGrounded;
    public bool IsClimbing => isClimbing;
    public bool IsWallSliding => isWallSliding;
    public bool CurrentlyDying => currentlyDying;

    public event Action<bool> OnGroundedChange;
    public event Action OnJumping, OnDoubleJumping, OnWallJumping;
    public event Action<bool> OnDashingChanged;
    public event Action<bool> OnGroundedChanged;
    public event Action<bool> OnCrouchingChanged;

    private Rigidbody2D pRbody;
    private BoxCollider2D pCollider;
    private PlayerInput pInput;
    [SerializeField] private GameObject pFacer;
    private Vector2 lastPos;
    private Vector2 pvelocity;
    private Vector2 speed;
    private int fixedFrame;

    [SerializeField] private UIManager uiManager;
    void Awake()
    {
        pRbody = GetComponent<Rigidbody2D>();
        pCollider = GetComponent<BoxCollider2D>();
        pInput = GetComponent<PlayerInput>();

        defaultColliderSize = pCollider.size;         //Manipulate for state change
        defaultColliderOffset = pCollider.offset;     //Manipulate for state change
    }

    private void Update() => GatherInput();

    void FixedUpdate()
    {
        if (currentlyDying == true) TeleportOnDeath();
        death();
        if (currentlyDying == true) return;



        fixedFrame++;
        frameClamp = moveClamp;

        //Calculate Velocity
        pvelocity = (pRbody.position - lastPos) / Time.fixedDeltaTime;
        lastPos = pRbody.position;

        AdjustStatsToState();

        RunCollisionChecks();
        RunWallCollisionCheck();
        RunCanClimbCollisionCheck();

        CalculateHorizontal();
        //CalculateVertical();
        CalculateWallSlide();
        CalculateJumpApex();
        CalculateGravity();
        CalculateJump();
        CalculateClimb();
        //CalculateDash();
        //CalculateCrouch();
        MasterStateSetter();
        OrientFacer();
        MoveCharacter();
        

    }

    #region GatherInput
    private void GatherInput()
    {
        Input = pInput.GatherInput();

        if (Input.DashDown) dashToConsume = true;
        if (Input.JumpDown)
        {
            lastJumpPressed = fixedFrame;
            jumpToConsume = true;
        }

    }
    #endregion

    #region Collisions
    [Header("COLLISION")] [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float detectionRayLength = 0.1f;
    private RaycastHit2D[] hitsDown = new RaycastHit2D[3];
    private RaycastHit2D[] hitsUp = new RaycastHit2D[1];
    private RaycastHit2D[] hitsLeft = new RaycastHit2D[1];
    private RaycastHit2D[] hitsRight = new RaycastHit2D[1];

    [SerializeField] private bool hittingCeiling, isGrounded, collideRight, collideLeft;

    private float timeSinceGrounded;

    // Checks for pre-Collision information
    private void RunCollisionChecks()
    {
        //Generate ray ranges
        var b = pCollider.bounds;

        //Ground

        var groundedCheck = RunDetection(Vector2.down, out hitsDown);
        collideLeft = RunDetection(Vector2.left, out hitsLeft);
        collideRight = RunDetection(Vector2.right, out hitsRight);
        hittingCeiling = RunDetection(Vector2.up, out hitsUp);


        if (isGrounded && !groundedCheck)
        {
            timeSinceGrounded = fixedFrame; //Only trigger when first leaving
            OnGroundedChange?.Invoke(false);
        }
        else if (!isGrounded && groundedCheck)
        {
            coyoteUseable = true; // Only trigger when first touching
            executedBufferedJump = false;
            doubleJumpUsable = true;
            canDash = true;
            OnGroundedChange?.Invoke(true);
            speed.y = 0;
        }

        isGrounded = groundedCheck;

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.BoxCastAll(b.center, b.size, 0, direction, detectionRayLength, groundLayer);

            foreach (var hit in hits)
            {
                if (hit.collider && !hit.collider.isTrigger) return true;
            }
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!pCollider) pCollider = GetComponent<BoxCollider2D>();
        {
            Gizmos.color = Color.red;
            var b = pCollider.bounds;
            b.Expand(detectionRayLength);

            Gizmos.DrawWireCube(b.center, b.size);
        }
    }

    #endregion

    #region Crouch

    [SerializeField, Header("Crouch")] private float crouchSizeModifier = 0.5f;
    [SerializeField] private float crouchSpeedModifier = 0.1f;
    [SerializeField] private int crouchSlowdownFrames = 50;
    [SerializeField] private float immediateCrouchSlowdownThreshold = 0.1f;
    private Vector2 defaultColliderSize, defaultColliderOffset;
    private float velocityOnCrouch;
    private bool isCrouching;
    private int frameStartedCrouching;

    private bool CanStand {
        get
        {
            var col = Physics2D.OverlapBox((Vector2)transform.position + defaultColliderOffset, defaultColliderSize * 0.95f, 0, groundLayer);
            return (col == null || col.isTrigger);
        }
    }

    
    void CalculateCrouch()
    {
        if (!allowCrouch) return;

        if (isCrouching)
        {
            var immediate = velocityOnCrouch <= immediateCrouchSlowdownThreshold ? crouchSlowdownFrames : 0;
            var crouchPoint = Mathf.InverseLerp(0, crouchSlowdownFrames, fixedFrame - frameStartedCrouching + immediate);
            frameClamp *= Mathf.Lerp(1, crouchSpeedModifier, crouchPoint);
        }

        if (isGrounded && Input.Y < 0 && !isCrouching)
        {
            isCrouching = true;
            OnCrouchingChanged?.Invoke(true);
            velocityOnCrouch = Mathf.Abs(pvelocity.x);
            frameStartedCrouching = fixedFrame;

            pCollider.size = defaultColliderSize * new Vector2(1, crouchSizeModifier);

            //lower the collider by the difference extent
            var difference = defaultColliderSize.y - (defaultColliderSize.y * crouchSizeModifier);
            pCollider.offset = -new Vector2(0, difference * 0.5f);
        }
        else if (!isGrounded || (Input.Y >= 0 && isCrouching))
        {
            //Detect obstruction in standing area. Add a 0.1 y buffer to avoid the ground
            if (!CanStand) return;
            isCrouching = false;
            OnCrouchingChanged?.Invoke(false);

            pCollider.size = defaultColliderSize;
            pCollider.offset = defaultColliderOffset;
        }

    }


    #endregion

    #region SetPlayerState
    [SerializeField, Header("STATE MANAGER")] public bool playerState1;
    [SerializeField] public bool playerState2;
    [SerializeField] public bool playerState3;
    [SerializeField] public bool playerState4;
    private float state1SizeModifer = 1f;
    private float state2SizeModifer = 0.75f;
    private float state3SizeModifer = 0.75f;
    private float state4SizeModifer = 0.5f;

    private void MasterStateSetter()
    {
        if (GetComponent<Health>().GetCurrentHealth >= 4) SetPlayerStateOne();
        if (GetComponent<Health>().GetCurrentHealth == 3) SetPlayerStateTwo();
        if (GetComponent<Health>().GetCurrentHealth == 2) SetPlayerStateThree();
        if (GetComponent<Health>().GetCurrentHealth <= 1) SetPlayerStateFour();
    }

    private void SetPlayerStateOne()
    {
        playerState1 = true;
        playerState2 = false;
        playerState3 = false;
        playerState4 = false;

        allowDoubleJump = true;
        pCollider.size = defaultColliderSize;
        pCollider.offset = defaultColliderOffset;
    }
    private void SetPlayerStateTwo()
    {
        playerState1 = false;
        playerState2 = true;
        playerState3 = false;
        playerState4 = false;

        allowDoubleJump = false;
        pCollider.size = defaultColliderSize * new Vector2(1, state2SizeModifer);

        var difference = defaultColliderSize.y - (defaultColliderSize.y * state2SizeModifer);
        pCollider.offset = -new Vector2(0, difference * 0.50f);
    }
    private void SetPlayerStateThree()
    {
        playerState1 = false;
        playerState2 = false;
        playerState3 = true;
        playerState4 = false;

        allowDoubleJump = false;
        pCollider.size = defaultColliderSize * new Vector2(1, state3SizeModifer);

        var difference = defaultColliderSize.y - (defaultColliderSize.y * state3SizeModifer);
        pCollider.offset = -new Vector2(0, difference * 0.50f);
    }
    private void SetPlayerStateFour()
    {
        playerState1 = false;
        playerState2 = false;
        playerState3 = false;
        playerState4 = true;

        allowDoubleJump = false;
        pCollider.size = defaultColliderSize * new Vector2(1, state4SizeModifer);

        var difference = defaultColliderSize.y - (defaultColliderSize.y * state4SizeModifer);
        pCollider.offset = -new Vector2(0, difference * 0.5f);
    }




    #endregion

    #region Horizontal

    [Header("WALKING")] [SerializeField] private float acceleration = 120;
    [SerializeField] private float moveClamp = 8;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float jumpApexBonus = 80;

    [SerializeField] private bool allowCreeping;

    private float frameClamp;

    private void CalculateHorizontal()
    {
        if (Input.X != 0)
        {
            //Set Horizontal Speed
            if (allowCreeping) speed.x = Mathf.MoveTowards(speed.x, frameClamp * Input.X, acceleration * Time.fixedDeltaTime);
            else speed.x += Input.X * acceleration * Time.fixedDeltaTime;

            //Clamped by max frame movement
            speed.x = Mathf.Clamp(speed.x, -frameClamp, frameClamp);

            //Apply bonus to apex of jump
            var apexBonus = Mathf.Sign(Input.X) * jumpApexBonus * apexPoint;
            speed.x += apexBonus * Time.fixedDeltaTime;
        }
        else
        {
            //No input. Slow character down
            speed.x = Mathf.MoveTowards(speed.x, 0, deceleration * Time.fixedDeltaTime);
        }

        if (!isGrounded && (speed.x > 0 && collideRight || speed.x < 0 && collideLeft))
        {
            //Prevents sticking to walls mid air
            speed.x = 0;
        }
        if (isClimbing)
        {
            speed.x *= 0.5f;
        }
    }
    #endregion

    #region Vertical

    [Header("VERTICAL MOVEMENT")] [SerializeField] private bool canVertical;
    [SerializeField] private float verticalAcceleration = 50f;
    private void CalculateVertical()
    {
        if (Input.Y != 0)
        {
            if (canVertical) speed.y += Input.Y * verticalAcceleration * Time.fixedDeltaTime;
            
            speed.y = Mathf.Clamp(speed.y, -frameClamp, frameClamp);

        }
        else
        {
            //No input. Slow character down
            speed.y = Mathf.MoveTowards(speed.y, 0, deceleration * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Gravity

    [Header("GRAVITY")] [SerializeField] private float fallClamp = -40f;
    [SerializeField] private float minFallSpeed = 50f;
    [SerializeField] private float maxFallSpeed = 50f;
    [SerializeField, Range(0, -10)] private float groundingForce = -1.5f;
    private float fallingSpeed;

    private void CalculateGravity()
    {
        if (isGrounded)
        {
            if (Input.X == 0) return;

            //Slopes
            speed.y = groundingForce;
            foreach (var hit in hitsDown)
            {
                if (hit.collider.isTrigger) continue;
                var slopePerp = Vector2.Perpendicular(hit.normal).normalized;
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeAngle != 0)
                {
                    speed.y = speed.x * -slopePerp.y;
                    speed.y += groundingForce;
                    break;
                }
            }
        }
        else
        {
            if (isWallSliding) return;
           
            //addd downward force while ascending if we ended jump to early
            var fallSpeed =  endedJumpEarly && speed.y > 0 ? fallingSpeed * jumpEndEarlyGravityModifier : fallingSpeed;

            //fall
            speed.y -= fallSpeed * Time.fixedDeltaTime;

            //clamp
            if (speed.y < fallClamp) speed.y = fallClamp;
        }
    }
    #endregion

    #region Jump

    [Header("JUMPING")] [SerializeField] private float jumpHeight = 18;
    [SerializeField] private float jumpApexThreshhold = 40f;
    [SerializeField] private int coyoteTimeThreshhold = 7;
    [SerializeField] private int jumpBuffer = 7;
    [SerializeField] private float jumpEndEarlyGravityModifier = 3;
    [SerializeField] private bool jumpToConsume;
    private bool coyoteUseable;
    private bool executedBufferedJump;
    private bool endedJumpEarly = true;
    private float apexPoint; //Becomes 1 at the apex of the jump
    private float lastJumpPressed = float.MinValue;
    [SerializeField] private bool doubleJumpUsable;
    [SerializeField] private bool canWallJump;
    [SerializeField] private float wallJumpForceHorizontal = 25f;
    private bool CanUseCoyote => coyoteUseable && !isGrounded && timeSinceGrounded + coyoteTimeThreshhold > fixedFrame;
    private bool HasBufferedJump => ((isGrounded && !executedBufferedJump) || cornerStuck) && lastJumpPressed + jumpBuffer > fixedFrame;
    private bool CanDoubleJump => allowDoubleJump && doubleJumpUsable && !coyoteUseable;
    private bool CanWallJump => isWallSliding && canWallJump;


    [SerializeField] private bool executedWallJump = false;
    private void CalculateJumpApex()
    {
        if (!isGrounded)
        {
            //Gets stronger the closer to the top of the jump
            apexPoint = Mathf.InverseLerp(jumpApexThreshhold, 0, Mathf.Abs(pvelocity.y));
            fallingSpeed = Mathf.Lerp(minFallSpeed, maxFallSpeed, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }
    }
    private void CalculateJump()
    {

        if (jumpToConsume && CanDoubleJump)
        {
            speed.y = jumpHeight;
            doubleJumpUsable = false;
            endedJumpEarly = false;
            jumpToConsume = false;
            OnDoubleJumping?.Invoke();
        }

        //Jump if: gorunded or within coyote threshold|| sufficient jump buffer || or if on wall

        if ((jumpToConsume && CanUseCoyote) || HasBufferedJump)
        {
            speed.y = jumpHeight;
            endedJumpEarly = false;
            coyoteUseable = false;
            jumpToConsume = false;
            timeSinceGrounded = fixedFrame;
            executedBufferedJump = true;
            OnJumping?.Invoke();
        }

        if (jumpToConsume && CanWallJump || HasBufferedJump)
        {
            speed.y = jumpHeight;
            speed.x = Input.X * wallJumpForceHorizontal;
            canWallJump = false;
            endedJumpEarly = false;
            jumpToConsume = false;
            executedWallJump = true;
            OnWallJumping?.Invoke();
        }


        //End the jump early if button released
        if (!isGrounded && !Input.JumpHeld && !endedJumpEarly && pvelocity.y > 0) endedJumpEarly = true;

        if (hittingCeiling && speed.y > 0) speed.y = 0;

    }



    #endregion

    #region Wall Slide
    [Header("WALL SLIDE")] [SerializeField] private LayerMask wallLayer;

    [SerializeField] private bool wallCheckLeft, wallCheckRight;
    [SerializeField] private bool isWallSliding;
    public float wallSlideSpeed = 5;

    void CalculateWallSlide()
    {
        if (playerState3 || playerState4) return;

        if (wallCheckLeft || wallCheckRight && !isGrounded && Input.X != 0)
        {

            Debug.Log("Touching wall");
            speed.x = 0;
            isGrounded = false;
            isWallSliding = true;
        }
        else isWallSliding = false;
        if (isWallSliding)
        {

            canWallJump = true;
            speed.x = 0;
            speed.y = -wallSlideSpeed;
        }
    }

    private void RunWallCollisionCheck()
    {
        //Generate ray ranges
        var b = pCollider.bounds;

        //Ground

        wallCheckRight = RunDetection(Vector2.right, out hitsRight);
        wallCheckLeft = RunDetection(Vector2.left, out hitsRight);

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.BoxCastAll(b.center, b.size, 0, direction, detectionRayLength, wallLayer);

            foreach (var hit in hits)
            {
                if (hit.collider && !hit.collider.isTrigger) return true;
            }
            return false;
        }
    }

    #endregion

    #region Climbing
    [Header("CLIMBING")] [SerializeField] private LayerMask climbLayer;

    [SerializeField] public bool isClimbing;
    [SerializeField] private bool climbCheckUp;

    void CalculateClimb()
    {
        if (climbCheckUp && Input.Y > 0)
        {
            speed.y = 0;
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }


    private void RunCanClimbCollisionCheck()
    {
        //Generate ray ranges
        var b = pCollider.bounds;


        climbCheckUp = RunDetection(Vector2.up, out hitsUp); ;

        bool RunDetection(Vector2 direction, out RaycastHit2D[] hits)
        {
            hits = Physics2D.BoxCastAll(b.center, b.size, 0, direction, detectionRayLength, climbLayer);

            foreach (var hit in hits)
            {
                if (hit.collider && !hit.collider.isTrigger) return true;
            }
            return false;
        }
    }

    #endregion

    #region Dash

    [Header("DASH")] [SerializeField] private float dashPower = 30;
    [SerializeField] private int dashLength = 6;
    [SerializeField] private float dashEndHorizontalMultiplier = 0.25f;
    private float startedDashing;
    private bool canDash;
    private Vector2 dashVelocity;

    private bool dashing;
    private bool dashToConsume;

    void CalculateDash()
    {
        if (!allowDash) return;

        if (dashToConsume && canDash)
        {
            var veloc = new Vector2(Input.X, isGrounded && Input.Y < 0 ? 0 : Input.Y).normalized;
            if (veloc == Vector2.zero) return;
            dashVelocity = veloc * dashPower;
            dashing = true;
            OnDashingChanged?.Invoke(true);
            canDash = false;
            startedDashing = fixedFrame;

            //strip external buildup
            forceBuildup = Vector2.zero;
        }

        if (dashing)
        {
            speed.x = dashVelocity.x;
            speed.y = dashVelocity.y;
            //cancel when the time is out or we've reached our max safe distance
            if (startedDashing + dashLength < fixedFrame)
            {
                dashing = false;
                OnDashingChanged?.Invoke(false);
                if (speed.y > 0) speed.y = 0;
                speed.x *= dashEndHorizontalMultiplier;
                if (isGrounded) canDash = true;
            }
        }

        dashToConsume = false;
    }

    #endregion

    #region Move

    //Cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        RawMovement = speed; //used externally
        var move = RawMovement * Time.fixedDeltaTime;

        //Apply effectors

        move += EvaluateEffectors();
        move += EvaluateForces();

        pRbody.MovePosition(pRbody.position + move);

        RunCornerPrevention();
    }

    #region Corner Stuck Prevention
    private Vector2 lastPosition;
    private bool cornerStuck;

    //allows walking and jumpoing while right on the corner of ledge
    void RunCornerPrevention()
    {
        cornerStuck = !isGrounded && lastPosition == pRbody.position && lastJumpPressed + 1 < fixedFrame;
        speed.y = cornerStuck ? 0 : speed.y;
        lastPosition = pRbody.position;
    }
    #endregion

    #endregion

    #region Effectors & Forces

    private readonly List<IPlayerEffector> usedEffectors = new List<IPlayerEffector>();
    /// <summary>
    /// for more passive force effects like moving platforms, underwater, wind etc
    /// </summary>
    /// <returns></returns>
    
    private Vector2 EvaluateEffectors()
    {
        var effectorDirection = Vector2.zero;
        //repeat this for other directions and possibly even area effectors. wind zones, underwater etc
        effectorDirection += Process(hitsDown);

        usedEffectors.Clear();
        return effectorDirection;

        Vector2 Process(IEnumerable<RaycastHit2D> hits)
        {
            foreach (var hit2D in hits)
            {
                if (!hit2D.transform) return Vector2.zero;
                if (hit2D.transform.TryGetComponent(out IPlayerEffector effector))
                {
                    if (usedEffectors.Contains(effector)) continue;
                    usedEffectors.Add(effector);
                    return effector.EvaluateEffector();
                }
            }

            return Vector2.zero;
        }
    }

    [Header("EFFECTORS")] [SerializeField] private float forceDecay = 1;
    private Vector2 forceBuildup;

    public void AddForce(Vector2 force, PlayerForce mode = PlayerForce.Burst, bool cancelMovement = true)
    {
        if (cancelMovement) speed = Vector2.zero;
        
        switch (mode)
        {
            case PlayerForce.Burst:
                speed += force;
                break;
            case PlayerForce.Decay:
                forceBuildup += force * Time.fixedDeltaTime;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private Vector2 EvaluateForces()
    {
        //Prevent Bouncing
        if (collideLeft || collideRight) forceBuildup.x = 0;
        if (isGrounded || hittingCeiling) forceBuildup.y = 0;

        var force = forceBuildup;

        forceBuildup = Vector2.MoveTowards(forceBuildup, Vector2.zero, forceDecay * Time.fixedDeltaTime);

        return force;
    }

    #endregion

    #region Misc
    private void OnValidate()
    {
        if (groundLayer.value == 0)
        {
            Debug.LogError("Controller ground mask is NULL, assign ground to controller");
        }
    }
    #endregion

    #region DirtyFacer

    void OrientFacer()
    {
        var inputPoint = Input.X;

        if (inputPoint > 0) pFacer.transform.localRotation = Quaternion.Euler(0, 0, 0);
        else if (inputPoint < 0) pFacer.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    #endregion

    #region TeleportOnDeath

    [SerializeField, Header("Death Teleport")] private Transform teleportPoint;
    [SerializeField] public bool currentlyDying = false;
    [SerializeField] public bool running = false;
    public void TeleportOnDeath()
    {
        if (!currentlyDying) return;
        
        if (running == false)
        {
            StartCoroutine(DeathDelay());
        }
        
    }

    private IEnumerator DeathDelay()
    {
        running = true;
        uiManager.Fader.SetActive(true);
        yield return new WaitForSeconds(5);
        transform.position = teleportPoint.position;
        currentlyDying = false;
        running = false;
        uiManager.Fader.SetActive(false);
    }

    #endregion

    #region StatControl

    void AdjustStatsToState()
    {

        if(playerState1 == true)
        {
            jumpHeight = 18;
            jumpApexBonus = 80;
            moveClamp = 8;
            acceleration = 120;
        }
        else if (playerState2 == true)
        {
            jumpHeight = 15;
            moveClamp = 7;
            acceleration = 120;
            jumpApexBonus = 80;
        }
        else if (playerState3 == true)
        {
            jumpHeight = 25;
            jumpApexBonus = 100;
            moveClamp = 5;
            acceleration = 120;
        }
        else if (playerState4 == true)
        {
            jumpHeight = 20;
            jumpApexBonus = 135;
            moveClamp = 13;
            acceleration = 175;
        }

    }


    #endregion


    void death()
    {
        if (GetComponent<Health>().GetCurrentHealth < 1)
        {
            uiManager.LoseContainer.SetActive(true);
        }
    }
}
