using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    [Header("MoveSpeed settings")]
    public float maxSpeed = 80f;
    public float maxAcceleration = 40f;
    public float drag = 40f;
    public float reactionMultiplier = 0.5f; 
    public float moveSpeed = 60f;
    public float dragMultiplier = 0.5f;
    public float slideDragMultiplier = 0.2f;

    [Header("Jump settings")]
    public float jumpVelocity = 25f;
    public float fallMultiplier = 7.5f;
    public float gravityMultiplier = 0.01f;
    public float airDragMultiplier = 0.05f;
    public float rayLength = 1;
    
    [Header("Swinging Settings")]
    public float swingingMaxSpeed = 100f;
    public float swingingAcceleration = 80f;
    public float swingingReactionMultiplier = 0.25f;
    public float disableGravitySpeed = 7.5f;
    public float swingingGravityTime = 20f;
    public float swingGravityModifier = 0.5f;

    [Header("Health and variables")]
    public float maxLives = 10f;
    public float lives = 10f;

    Rigidbody2D rb;
    GameObject player;
    float currentMaxSpeed;
    bool turned = false;
    int directionSwinging = 0;
    
    //public Renderer renderer;
    public float flashTime;  
    private Color originalColor;

    bool chainGravity = false;
    bool prevGrounded = false;
    Vector2 playerCheckpointPosition = new Vector2(0, 0);
    float timeLeft;

    AudioSource audioSource;
    public AudioClip playerFootstep;

    public Animator animator;

    public LayerMask groundLayer;

    private CheckPoint activeCheckpoint;
    private SpriteRenderer spriteRenderer;
    private ThrowHook throwHook;
    private Collider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        playerCheckpointPosition = transform.position;
        playerFootstep = Resources.Load<AudioClip>("footstep");

        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        throwHook = GetComponent<ThrowHook>();
        playerCollider = GetComponent<Collider2D>();


    }

    private void Awake()
    {
        currentMaxSpeed = maxSpeed;
    }

    // Set swinging bool for the animation player
    void Animation()
    {
        if (IsSwinging())
        {
            animator.SetBool("IsSwinging", true);
        }
        else
        {
            animator.SetBool("IsSwinging", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MovementJump();
        Audio();
        Animation();
        Movement();

    }

    // FixedUpdate is called per fixed Timestep (0.02 seconds / 50 times per second)
    void FixedUpdate()
    {
        PlayerTurn();
        if (IsSwinging())
        {
            Rotation();
        }
        HitGround();
    }

    // Play footstep audio when walking
    public void Audio()
    {
        if ((rb.velocity.x < -1 || rb.velocity.x > 1))
        {
            if (!audioSource.isPlaying && IsGrounded())
            {
                audioSource.clip = playerFootstep;
                audioSource.volume = Random.Range(0.8f, 1f);
                audioSource.pitch = Random.Range(0.8f, 1.1f);
                audioSource.Play();
            }
        }
    }

    // Function for handling what variables to give to the playerMovement function
    private void Movement()
    {
        // Give different values for movement if the player is swinging or not
        if (IsSwinging()) 
        {
            PlayerMovement(swingingAcceleration, swingingReactionMultiplier, swingingMaxSpeed, dragMultiplier);
        }
        else
        {
            // Make sure to lower currentMaxSpeed after releasing the swing so the player can't accelerate further in the air
            if (rb.velocity.x > maxSpeed || rb.velocity.x < -maxSpeed)
            {
                if (rb.velocity.x > 0)
                    currentMaxSpeed = rb.velocity.x;
                if (rb.velocity.x < 0)
                    currentMaxSpeed = -rb.velocity.x;
            }

            PlayerMovement(maxAcceleration, reactionMultiplier, maxSpeed, dragMultiplier);
        }
    }

    /// <summary>
    /// Checks if the player lands on the ground this frame
    /// </summary>
    private void HitGround()
    {
        if (IsGrounded() && !prevGrounded)
        {
            AudioManager.PlaySound("Land");
        }
        prevGrounded = IsGrounded();
    }

    // Get the last position where the player was grounded, used for the checkpoints
    public void CheckpointReached(CheckPoint checkpoint)
    {
        //RaycastHit2D groundRay = GroundrayCast(GetComponent<Collider2D>().bounds.size.x * 0.5f);

        // Change to colliding with checkpoint
        playerCheckpointPosition = checkpoint.transform.position;

        if (activeCheckpoint != null)
        {
            activeCheckpoint.currentActive = false;
        }

        activeCheckpoint = checkpoint;
    }
    
    // Send a raycast to the ground
    private RaycastHit2D GroundrayCast(float xOffset)
    {
        Vector2 position = (Vector2)transform.position + playerCollider.bounds.size.y * Vector2.down / 2 - new Vector2(0, 0.2f) + new Vector2(xOffset, 0);
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, rayLength);

        return hit;
    }

    // Give faster speed while attached to the chain
    private bool IsSwinging()
    {
        if (throwHook.firstHook != null && !IsGrounded())
        {
            if (throwHook.firstHook.GetComponent<ChainScript>().isFlexible)
            {
                if (directionSwinging == 0)
                {
                    if (turned)
                        directionSwinging = 1;
                    else
                        directionSwinging = 2;
                }
                return true;
            }
        }
        directionSwinging = 0;
        spriteRenderer.flipX = false;
        return false;

    }
    
    // function to preserve max speed and slowly decrease it depending on if on ground or not
    private float[] Momentum(float speed, float maximumSpeed, float goalMaximumSpeed, float dragMultiplier)
    {
        float finalDrag;

        // Change the rate at which speed is lost based on if the player is on the ground or not
        if (IsGrounded())
        {
            finalDrag = (drag * dragMultiplier);
        }
        else
        {
            // Increase airdrag while not holding the movement keys
            if (!Input.GetButton("Right") && !Input.GetButton("Left"))
            {
                finalDrag = (drag * airDragMultiplier * 3f);
            }
            else
            {
                finalDrag = (drag * airDragMultiplier);
            }
        }

        // if no movement keys are pressed lower or increase speed by the drag value until it goes back to 0
        if (speed > 0)
        {
            speed -= finalDrag * Time.deltaTime;
            if (speed < 0.1)
                speed = 0;
        }
        else if (speed < 0)
        {
            speed += finalDrag * Time.deltaTime;
            if (speed > -0.1)
                speed = 0;
        }

        
        // lower the maxspeed at the same rate as speed
        if (maximumSpeed > goalMaximumSpeed)
            maximumSpeed -= finalDrag * Time.deltaTime;

        // Return the new speed and maximumSpeed
        float[] newValues = {speed, maximumSpeed};
        return newValues;
    }

    // Function to handle player movement
    public void PlayerMovement(float currentAcceleration, float currentReactionMultiplier, float goalMaxSpeed, float dragMultiplier)
    {
        // Allow changing of the current x velocity.
        float speed = rb.velocity.x;

        if (currentMaxSpeed < goalMaxSpeed)
        {
            currentMaxSpeed = goalMaxSpeed;
        }

        // Add acceleration to speed if the keys for going right or left are pressed.
        if (Input.GetButton("Right"))
        {
            if (speed < 0)
            {
                speed += ((currentAcceleration + currentAcceleration * currentReactionMultiplier) * Time.deltaTime);
            }
            else
            {
                speed += (currentAcceleration * Time.deltaTime);
            }
        }
        else if (Input.GetButton("Left"))
        {
            if (speed > 0)
            {
                speed -= ((currentAcceleration + currentAcceleration * currentReactionMultiplier) * Time.deltaTime);
            }
            else
            {
                speed -= (currentAcceleration * Time.deltaTime);
            }

        }

        // lower movement speed
        float[] newValues = Momentum(speed, currentMaxSpeed, goalMaxSpeed, dragMultiplier);

        speed = newValues[0];
        currentMaxSpeed = newValues[1];
        
        // Make sure the player can't go above the maximum speed limit
        if (speed > 0)
            speed = Mathf.Min(speed, currentMaxSpeed);

        if (speed < 0)
            speed = Mathf.Max(speed, -currentMaxSpeed);

        rb.velocity = new Vector2(speed, rb.velocity.y);
    }


    void PlayerTurn()
    {
        // Turn the player around if they are moving in one direction above a certain speed
        if (rb.velocity.x > 0)
        {
            turned = false;
        }
        if (rb.velocity.x < 0)
        {
            turned = true;
        }

        if (!turned)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }

    bool IsGrounded()
    {
        // Probably get layers set up so you don't need to hardcode what not to jump on.
        RaycastHit2D hit = GroundrayCast(playerCollider.bounds.size.x * 0.3f - 0.1f);
        RaycastHit2D hit2 = GroundrayCast(playerCollider.bounds.size.x * -0.3f + 0.1f);
        if (hit.collider != null || hit2.collider != null)
        {
            return true;
        }

        return false;
    }

    public void MovementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump") && (IsGrounded()))
        {
            AudioManager.PlaySound("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        // disable gravity for a small amount of time after releasing the chain to maintain momentum    
        if (chainGravity)
        {
            // Remove chainGravity after timeLeft reaches 0 when lowering by 60 per second
            timeLeft -= 60 * Time.deltaTime;
            if (timeLeft < 0)
            {
                chainGravity = false;
                timeLeft = swingingGravityTime;
            }
        }
        else
        {
            // Apply a modifier to gravity while the player is swinging
            float modifier;

            if (IsSwinging())
            {
                modifier = swingGravityModifier;
            }
            else
            {
                modifier = 1;
            }

            //gravity
            rb.velocity += (Vector2.up * Physics2D.gravity * ((gravityMultiplier * modifier)  - 1)) * Time.deltaTime;

            // Speed up falling when releasing the jump button while still jumping
            if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += (Vector2.up * Physics2D.gravity * ((fallMultiplier * modifier) - 1)) * Time.deltaTime;
            }
        }
    }

    // When vertical velocity is above a point while swinging, disable gravity for a short period of time
    public void ChainJump()
    {
        if (rb.velocity.y > disableGravitySpeed || rb.velocity.y < -disableGravitySpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            timeLeft = swingingGravityTime;
            chainGravity = true;
        }
    }
    
    // Function to lower the player lives when getting hit and reset them back to a checkpoint
    public void TakeDamage(float amount)
    {
        AudioManager.PlaySound("PlayerHit");
        lives -= amount;

        if (throwHook.firstHook != null)
            Destroy(throwHook.firstHook);
        transform.position = playerCheckpointPosition;
    }

    // Rotate the player towards the hook anchor when swinging
    void Rotation()
    {
        var playerToHookDirection = (throwHook.firstHook.transform.position - (Vector3)transform.position).normalized;
        var angle = Mathf.Atan2(playerToHookDirection.y, playerToHookDirection.x) * Mathf.Rad2Deg;

        if (directionSwinging == 1)
        {
            spriteRenderer.flipX = true;
        }
        else if (directionSwinging == 2)
        {
            spriteRenderer.flipX = false;
        }
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        var nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject node in nodes)
        {
            node.transform.rotation = transform.rotation;
        }

    }

}
