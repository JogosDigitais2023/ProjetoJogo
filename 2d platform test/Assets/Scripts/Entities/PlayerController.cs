using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private LineRenderer line;
    private DistanceJoint2D distanceJoint;
    private ClosestTargetDetector closestTargetDetector;
    public bool IsGliding { get; set; } = false;
    public bool IsSwitching { get; set; } = false;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 27f;
    [SerializeField] private float m_FallSpeed = 0f;
    [SerializeField] private LayerMask jumpableGround;

    private float dirX;
    public DashState dashState;
    private CharacterState characterState;
    public Vector2 savedVelocity;
    public float dashTimer = 0;
    public float maxDash = 1f;
    public float dashSpeed = 1;
    
    [SerializeField] public float switchTimer = 0.1f;

    // Animações
    private string idle;
    private string running;
    private string jumping;
    private string falling;
    private string currentState;
    private string state;

    //Inputs
    const string Jump = "Jump";
    const string Switch = "Fire1";
    const string Skill1 = "Fire2";
    const string Skill2 = "Fire3";

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
        UpdateCharacterState();
        Movement();
        Respawn();
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (rigidBody.velocity.x > 0f)
        {
            state = running;
            sprite.flipX = false;
        }
        else if (rigidBody.velocity.x < 0f)
        {
            state = running;
            sprite.flipX = true;
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
            state = "Transformation";
        }

        if (IsGliding)
        {
            state = "Mage_Gliding";
        }
        SetAnimation(state);
    }

    void SetAnimation(string newState)
    {
        if (currentState != newState)
        {
            animator.Play(newState);
            currentState = newState;
        }
    }

    void UpdateCharacterState()
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
                Dash();
                Swing();

                idle = "Slinger_Idle";
                running = "Slinger_Running";
                jumping = "Slinger_Jumping";
                falling = "Slinger_Falling";
                break;
        }
    }

    void SwitchCharacter()
    {
        if (Input.GetButtonDown(Switch))
        {
            IsSwitching = true;
            if (characterState == CharacterState.mage)
            {
                characterState = CharacterState.slinger;
                Debug.Log("slinger");
            }
            else if (characterState == CharacterState.slinger)
            {
                characterState = CharacterState.mage;
                Debug.Log("mage");
            }
            Debug.Log("a");

            if(IsSwitching)
            {
                // Freeze();
            }

            Invoke("EndSwitching", switchTimer);
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
        dirX = Input.GetAxisRaw("Horizontal");
        ////runner mode
        rigidBody.velocity = new Vector2(moveSpeed * dashSpeed, rigidBody.velocity.y);

        //normal mode
        //rigidBody.velocity = new Vector2(moveSpeed * dirX * dashSpeed, rigidBody.velocity.y);
        if (Input.GetButtonDown(Jump) && IsGrounded())
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }

    public void Glide() 
    {
        if (IsGliding && rigidBody.velocity.y < 0f && Mathf.Abs(rigidBody.velocity.y) > m_FallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Sign(rigidBody.velocity.y) * m_FallSpeed);

        if (Input.GetButtonDown(Skill1) && !IsGrounded())
        {
            IsGliding = true;
        }

        if (Input.GetButtonUp(Skill1) && !IsGrounded() || IsGrounded())
        {
            IsGliding = false;
        }
    }

    public void Dash()
    {
        switch (dashState)
        {
            case DashState.Ready:
                if (Input.GetButtonDown(Skill1) && IsGrounded())
                {
                    savedVelocity = rigidBody.velocity;
                    //rigidBody.velocity = new Vector2(rigidBody.velocity.x * 3f, rigidBody.velocity.y);
                    dashSpeed = 3;
                    dashState = DashState.Dashing;
                    Debug.Log("ready");
                }
                break;
            case DashState.Dashing:
                dashTimer += Time.deltaTime * 3;
                if (dashTimer >= maxDash)
                {
                    dashTimer = maxDash;
                    //rigidBody.velocity = savedVelocity;
                    dashSpeed = 1;
                    dashState = DashState.Cooldown;
                    Debug.Log("dashing");
                }
                break;
            case DashState.Cooldown:
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.Ready;
                    Debug.Log("cooldown");
                }
                break;
        }
    }

    public void Swing()
    {
        if (Input.GetButtonDown(Skill2) && closestTargetDetector.target != null)
        {
            Vector2 targetPosition = closestTargetDetector.target.transform.position;

            line.SetPosition(0, targetPosition);
            line.SetPosition(1, transform.position);
            distanceJoint.connectedAnchor = targetPosition;
            distanceJoint.enabled = true;
            line.enabled = true;
            closestTargetDetector.arrow.SetActive(false);
            Debug.Log("swingin'");
        }
        else if (Input.GetButtonUp(Skill2))
        {
            distanceJoint.enabled = false;
            line.enabled = false;
            Debug.Log("not swingin'");
        }

        if (distanceJoint.enabled)
        {
            line.SetPosition(1, transform.position);
        }
    }

    private void Freeze()
    {
        Time.timeScale = 0;
    }

    private void Unfreeze()
    {
        Time.timeScale = 1;
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
    mage, slinger 
}