using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    [Header("MoveSpeed settings")]
    public float maxSpeed = 80f;
    public float swingingMaxSpeed = 100f;
    public float maxAcceleration = 40f;
    public float swingingAcceleration = 80f;
    public float drag = 40f;
    public float reactionMultiplier = 0.5f;
    public float swingingReactionMultiplier = 0.25f;

    public float moveSpeed = 60f;
    float currentMaxSpeed;

    public float hillMultiplier = 300f;

    bool turned = false;

    public float dragMultiplier = 0.5f;
    public float slideDragMultiplier = 0.2f;
    public float airDragMultiplier = 0.05f;

    Rigidbody2D rb;

    [Header("Jump settings")]
    public float jumpVelocity = 25f;
    public float fallMultiplier = 7.5f;
    public float gravityMultiplier = 0.01f;
    public float rayLength = 1;
    public LayerMask groundLayer;

    public float maxHealth = 10f;
    public float health = 10f;

    //public Renderer renderer;
    public float flashTime;

    private Color originalColor;

    GameObject player;

    public Sprite sprite;

    bool chainGravity = false;

    public float defaultTime = 15f;
    float timeLeft;

    bool prevGrounded = false;

    Vector2 playerLastGroundedPosition = new Vector2(0, 0);
    private Vector2 jump;

    int directionSwinging = 0;

    AudioSource audioSource;
    public static AudioClip playerFootstep;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //originalColor = renderer.GetComponent<SpriteRenderer>().color;
        player = GameObject.FindGameObjectWithTag("Player");

        playerLastGroundedPosition = transform.position;
        playerFootstep = Resources.Load<AudioClip>("footstep");

        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        currentMaxSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MovementJump();
        Audio();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        PlayerTurn();
        if (IsSwinging())
        {
            Rotation();
        }
        HitGround();
    }

    [System.Obsolete]
    public void Audio()
    {
        if ((rb.velocity.x < -1 || rb.velocity.x > 1))
        {
            if (!audioSource.isPlaying && IsGrounded())
            {
                audioSource.clip = playerFootstep;
                audioSource.volume = Random.RandomRange(0.8f, 1f);
                audioSource.pitch = Random.RandomRange(0.8f, 1.1f);
                audioSource.Play();
            }
        }
    }

    private void Movement()
    {
        if (IsSwinging()) 
        {
            RevisedPlayerMovement(swingingAcceleration, swingingReactionMultiplier, swingingMaxSpeed, dragMultiplier);
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

            RevisedPlayerMovement(maxAcceleration, reactionMultiplier, maxSpeed, dragMultiplier);
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

    public void LastGroundedPosition()
    {
        //RaycastHit2D groundRay = GroundrayCast(GetComponent<Collider2D>().bounds.size.x * 0.5f);

        // Change to colliding with checkpoint
        if (IsGrounded())
        {
            playerLastGroundedPosition = transform.position;
        }
    }
    
    private RaycastHit2D GroundrayCast(float xOffset)
    {
        Vector2 position = (Vector2)transform.position + GetComponent<Collider2D>().bounds.size.y * Vector2.down / 2 - new Vector2(0, 0.2f) + new Vector2(xOffset, 0);
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, rayLength);

        return hit;
    }

    // Give faster speed while attached to the chain
    private bool IsSwinging()
    {
        if (GetComponent<ThrowHook>().firstHook != null)
        {
            if (GetComponent<ThrowHook>().firstHook.GetComponent<ChainScript>().isFlexible)
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
        GetComponentInChildren<SpriteRenderer>().flipX = false;
        return false;

    }

    // Debug function
    public void TestDamage()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeDamage(1);
        }
    }
    
    //function to preserve max speed and slowly decrease it depending on if on ground or not
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
            finalDrag = (drag * airDragMultiplier);
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

    public void RevisedPlayerMovement(float currentAcceleration, float currentReactionMultiplier, float goalMaxSpeed, float dragMultiplier)
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
        RaycastHit2D hit = GroundrayCast(GetComponent<Collider2D>().bounds.size.x * 0.3f - 0.1f);
        RaycastHit2D hit2 = GroundrayCast(GetComponent<Collider2D>().bounds.size.x * -0.3f + 0.1f);
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
            jump = new Vector2(0, jumpVelocity);
            rb.AddForce(jump, ForceMode2D.Impulse);
        }

        // disable gravity while swinging, and also for a small amount of time after releasing the chain to maintain momentum
        if (chainGravity || (IsSwinging() && (Input.GetButton("Right") || Input.GetButton("Left"))))
        {
            if (chainGravity)
            {
                timeLeft -= 60 * Time.deltaTime;
                if (timeLeft < 0)
                {
                    chainGravity = false;
                    timeLeft = defaultTime;
                }
            }
        }
        else
        {
            //gravity
            rb.velocity += (Vector2.up * Physics2D.gravity * (gravityMultiplier - 1)) * Time.deltaTime;

            // Speed up falling when releasing the jump button
            if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += (Vector2.up * Physics2D.gravity * (fallMultiplier - 1)) * Time.deltaTime;
            }
        }
    }

    public void ChainJump()
    {
        if (rb.velocity.y > 7.5f || rb.velocity.y < -7.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            timeLeft = defaultTime;
            chainGravity = true;
        }
    }
    


    public void TakeDamage(float amount)
    {
        AudioManager.PlaySound("PlayerHit");
        health = Mathf.Clamp(health -= amount, 0, maxHealth);

        if (GetComponent<ThrowHook>().firstHook != null)
            Destroy(GetComponent<ThrowHook>().firstHook);
        transform.position = playerLastGroundedPosition;
        //FlashRed();
    }

    void FlashRed()
    {
        //renderer.GetComponent<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", flashTime);
    }

    void ResetColor()
    {
        //renderer.GetComponent<SpriteRenderer>().color = originalColor;
    }

    void Rotation()
    {
        var playerToHookDirection = (GetComponent<ThrowHook>().firstHook.transform.position - (Vector3)transform.position).normalized;
        var angle = Mathf.Atan2(playerToHookDirection.y, playerToHookDirection.x) * Mathf.Rad2Deg;

        if (directionSwinging == 1)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
        else if (directionSwinging == 2)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        var nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject node in nodes)
        {
            node.transform.rotation = transform.rotation;
        }

    }

}
