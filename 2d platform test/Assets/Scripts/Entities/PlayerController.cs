using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Componentes
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private LineRenderer line;
    private DistanceJoint2D distanceJoint;
    private ClosestTargetDetector closestTargetDetector;
    public GameOverScreen GameOverScreen;

    // Checadores de estado
    public bool IsGliding { get; set; } = false;
    public bool IsSwitching { get; set; } = false;
    public bool IsDashing { get; set; } = false;
    public bool IsJumping { get; set; } = false;
    public bool IsSwinging { get; set; } = false;

    // Par�metros de jogo
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private float canJumpTimer = 0.1f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 27f;
    [SerializeField] private float m_FallSpeed = 0f;
    [SerializeField] private float switchTimer = 0.1f;
    [SerializeField] private float cutJumpHeight = 0.5f;
    [SerializeField] private float maxSwing = 1f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float decceleration = 1f;
    [SerializeField] private float velPower = 1f;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private float swingBoost = 1.5f;
    [SerializeField] private GameObject pauseMenu;

    // Vari�veis auxiliares
    private float dirX;
    private Vector2 savedVelocity;
    private float swingTimer = 0f;
    private float dashTimer = 0;
    private float maxDash = 1f;
    private float lastJumpedTime = 0;
    private float lastGroundedTime = 0;
    private bool canBreakWall = false;
    
    // Enums
    private DashState dashState;
    public CharacterState characterState;

    // Anima��es
    private string idle;
    private string running;
    private string jumping;
    private string falling;
    private string swinging;
    private string dashing = "Warrior_Dashing";
    private string gliding = "Mage_Gliding";
    private string switching = "Transformation";
    private string currentState;
    private string state;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        closestTargetDetector = GetComponent<ClosestTargetDetector>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(!pauseMenu.gameObject.activeSelf) 
        {
            UpdateCharacterState();
            Movement();
            Jump();
            Respawn();
            UpdateAnimationState();
        }
    }

    private void UpdateAnimationState()
    {
        if (rigidBody.velocity.x > 0.1f)
        {
            state = running;
            sprite.flipX = false;
        }
        else if (rigidBody.velocity.x < 0.1f)
        {
            state = running;
            //sprite.flipX = true;
        }
        else
        {
            state = idle;
        }

        if (rigidBody.velocity.y > 0.1f)
        {
            state = jumping;
        }
        else if (rigidBody.velocity.y < -0.1f)
        {
            state = falling;
        }

        if (IsSwitching)
        {
            state = switching;
        }

        if (IsGliding)
        {
            state = gliding;
        }

        if (IsDashing && IsGrounded())
        {
            state = dashing;
        }
        SetAnimation(state);
    }

    private void SetAnimation(string newState)
    {
        if (currentState != newState)
        {
            animator.Play(newState);
            currentState = newState;
        }
    }

    private void UpdateCharacterState()
    {
        SwitchCharacter();
        switch (characterState)
        {
            case CharacterState.mage:
                Glide();

                idle = "Mage_Idle";
                running = "Mage_Running";
                jumping = "Mage_Jumping";
                falling = "Mage_Falling";
                break;
            case CharacterState.slinger:
                Swing();

                idle = "Slinger_Idle";
                running = "Slinger_Running";
                jumping = "Slinger_Jumping";
                falling = "Slinger_Falling";
                break;
            case CharacterState.warrior:
                Dash();

                idle = "Warrior_Idle";
                running = "Warrior_Running";
                jumping = "Warrior_Jumping";
                falling = "Warrior_Falling";
                break;
        }
    }

    private void SwitchCharacter()
    {
        if (!IsDashing && !IsGliding && !IsSwinging && !IsSwitching)
        {
            if (Input.GetButtonDown("Switch1") && characterState != CharacterState.mage)
            {
                characterState = CharacterState.mage;
                IsSwitching = true;
                Debug.Log(characterState);

                Invoke("EndSwitching", switchTimer);
            }
            if (Input.GetButtonDown("Switch2") && characterState != CharacterState.slinger)
            {
                characterState = CharacterState.slinger;
                IsSwitching = true;
                Debug.Log(characterState);

                Invoke("EndSwitching", switchTimer);
            }
            if (Input.GetButtonDown("Switch3") && characterState != CharacterState.warrior)
            {
                characterState = CharacterState.warrior;
                IsSwitching = true;
                Debug.Log(characterState);

                Invoke("EndSwitching", switchTimer);
            }
        }
    }

    private void EndSwitching()
    {
        IsSwitching = false;
    }

    private void Respawn()
    {
        if (Input.GetKeyDown("return"))
        {
            transform.position = new Vector3(-6.33f, -0.65f, 0);
        }
    }

    private void Movement()
    {
        float targetSpeed = moveSpeed;
        float speedDif = targetSpeed - rigidBody.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > rigidBody.velocity.x) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rigidBody.AddForce(movement * Vector2.right);
        // runner mode
        // rigidBody.velocity = new Vector2(moveSpeed * dashSpeed, rigidBody.velocity.y);
        // normal mode
        // dirX = Input.GetAxisRaw("Horizontal");
        // rigidBody.velocity = new Vector2(moveSpeed * dirX * dashSpeed, rigidBody.velocity.y);
    }

    private void Jump()
    {
        // print("gt: " + lastGroundedTime + " deltaTime: " + Time.deltaTime + " " + Input.GetButtonDown("Jump"));
        lastGroundedTime -= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && (lastGroundedTime > 0))
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpedTime = canJumpTimer;
            AudioManager.Instance.PlaySFX("Jump");
        }

        if (Input.GetButtonUp("Jump") && rigidBody.velocity.y > 0.1f)
        {
            rigidBody.AddForce(Vector2.down * rigidBody.velocity.y * (1- cutJumpHeight), ForceMode2D.Impulse);
        }

        if (IsGrounded())
        {
            lastGroundedTime = canJumpTimer;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }

    private void Glide() 
    {
        if (IsGliding && rigidBody.velocity.y < 0f && Mathf.Abs(rigidBody.velocity.y) > m_FallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Sign(rigidBody.velocity.y) * m_FallSpeed);

        if (Input.GetButtonDown("Skill1") && !IsGrounded())
        {
            IsGliding = true;
            AudioManager.Instance.PlaySFX("Glide");
        }

        if (Input.GetButtonUp("Skill1") && !IsGrounded() || IsGrounded())
        {
            IsGliding = false;
        }
    }

    private void Dash()
    {
        switch (dashState)
        {
            case DashState.Ready:
                if (Input.GetButtonDown("Skill1") && IsGrounded())
                {
                    //savedVelocity = rigidBody.velocity;
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x * dashSpeed, rigidBody.velocity.y);
                    IsDashing = true;
                    dashState = DashState.Dashing;
                    AudioManager.Instance.PlaySFX("Dash");
                    Debug.Log("ready");
                }
                break;
            case DashState.Dashing:
                dashTimer += Time.deltaTime * 3;
                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    //rigidBody.velocity = savedVelocity;
                    dashState = DashState.Cooldown;
                    IsDashing = false;
                    Debug.Log("dashing");
                }
                break;
            case DashState.Cooldown:
                dashTimer -= Time.deltaTime * 2;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                    IsDashing = false;
                    Debug.Log("cooldown");
                }
                break;
        }

        if(IsDashing && canBreakWall)
        {
            Destroy(closestTargetDetector.targetObject);
            AudioManager.Instance.PlaySFX("Wall Break");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (closestTargetDetector.target != null)
        {
            if (closestTargetDetector.target.tag == "Wall" && collision.gameObject.tag == "Wall") 
            { 
                canBreakWall = true; 
            }
        }

        if (collision.gameObject.tag == "DeathZone")
        {
            GameOverScreen.Setup();
        }

        if (collision.gameObject.tag == "FinishIntro")
        {
            SceneManager.LoadScene("RinoLevel");
        }

        if (collision.gameObject.tag == "enemy")
        {
            GameOverScreen.Setup();
        }

        if (collision.gameObject.tag == "FinishRino")
        {
            SceneManager.LoadScene("LionLevel");
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            canBreakWall = false;
        }
    }

    private void Swing()
    {
        if (Input.GetButtonDown("Skill1") && closestTargetDetector.target != null && closestTargetDetector.target.position.x > rigidBody.position.x && closestTargetDetector.target.tag == "Swinger" && !IsGrounded())
        {
            Vector2 targetPosition = closestTargetDetector.target.transform.position;
            line.SetPosition(0, targetPosition);
            line.SetPosition(1, transform.position);
            distanceJoint.connectedAnchor = targetPosition;
            distanceJoint.enabled = true;
            line.enabled = true;
            closestTargetDetector.arrow.SetActive(false);
            IsSwinging = true;
            Debug.Log("swingin'");
            AudioManager.Instance.PlaySFX("Swing");
        }
        else if ((Input.GetButtonUp("Skill1") || swingTimer < 0 || IsGrounded()) && IsSwinging)
        {
            distanceJoint.enabled = false;
            line.enabled = false;
            IsSwinging = false;
            if(!IsGrounded()) {
                rigidBody.AddForce(swingBoost * rigidBody.velocity, ForceMode2D.Impulse);
            }
            Debug.Log("not swingin'");
        }

        if (!IsSwinging) swingTimer = maxSwing;

        if (distanceJoint.enabled)
        {
            line.SetPosition(1, transform.position);
            distanceJoint.distance -= Time.deltaTime * 3;
            swingTimer -= Time.deltaTime;
        }
    }
}

public enum DashState
{
    Ready,
    Dashing,
    Cooldown
}

public enum CharacterState 
{ 
    mage, slinger, warrior 
}