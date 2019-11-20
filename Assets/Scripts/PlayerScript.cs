using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    [Header("MoveSpeed settings")]
    public float maxSpeed = 80f;
    public float sprintMaxSpeed = 100f;
    public float maxAcceleration = 40f;
    public float sprintAcceleration = 80f;
    public float drag = 40f;
    public float reactionMultiplier = 0.5f;
    public float sprintReactionMultiplier = 0.25f;

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

    public Renderer renderer;
    public float flashTime;

    private Color originalColor;

    bool sprinting = false;
    bool crouching = false;
    bool sliding = false;

    GameObject player;

    public Sprite crouchSprite;
    public Sprite sprite;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalColor = renderer.GetComponent<MeshRenderer>().material.color;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Awake()
    {
        currentMaxSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        MovementJump();
        PlayerTurn();
        PlayerHealth();

        // debug
        TestDamage();
        Debug.Log(health);

        Debug.DrawLine((Vector2)transform.position + GetComponent<Collider2D>().bounds.size.y * Vector2.down / 2 - new Vector2(0, 0.1f), ((Vector2)transform.position + GetComponent<Collider2D>().bounds.size.magnitude * Vector2.down / 2) + (Vector2.down * rayLength), Color.green);
    }

    private void Movement()
    {
        if (GetComponent<ThrowHook>().active && !IsGrounded())
        {
            float playerMovement = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(playerMovement * Time.deltaTime * moveSpeed + rb.velocity.x, rb.velocity.y);
        }
        else
        {
            // Don't use the normal movement when crouching
            if (!Crouch())
            {
                if (Sprint())
                    RevisedPlayerMovement(sprintAcceleration, sprintReactionMultiplier, sprintMaxSpeed, dragMultiplier);
                else
                    RevisedPlayerMovement(maxAcceleration, reactionMultiplier, maxSpeed, dragMultiplier);
            } 
            else
            {
                PlayerSlide(slideDragMultiplier);
            }
        }

    }

    private RaycastHit2D GroundrayCast()
    {
        Vector2 position = (Vector2)transform.position + GetComponent<Collider2D>().bounds.size.magnitude * Vector2.down;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + GetComponent<Collider2D>().bounds.size.y * Vector2.down / 2 - new Vector2(0, 0.1f), direction, rayLength);

        return hit;
    }

    private bool Sprint()
    {
        if (Input.GetButton("Run"))
            return true;
        return false;
    }

    private bool Crouch()
    {
        if (Input.GetButton("Crouch") && IsGrounded())
            return true;

        GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        return false;
    }

    private void PlayerSlide(float dragMultiplier)
    {
        float speed = rb.velocity.x;
        GetComponentInChildren<SpriteRenderer>().sprite = crouchSprite;

        RaycastHit2D hit = GroundrayCast();
        if (hit.collider != null)
        {
            if (hit.normal.x < 1 && hit.normal.x > -1 && hit.normal.x != 0)
            {
                if (hit.normal.x > 0 && speed > -1)
                {
                    speed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                    if (speed > currentMaxSpeed)
                        currentMaxSpeed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                }

                if (hit.normal.x < 0 & speed < 1)
                {
                    speed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                    if (speed < -currentMaxSpeed)
                        currentMaxSpeed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                }
            }
        }

        // lower movement speed
        float[] newValues = Momentum(speed, currentMaxSpeed, maxSpeed, dragMultiplier);

        speed = newValues[0];
        currentMaxSpeed = newValues[1];

        // catch up maxspeed drag when sliding TODO: find a better way of doing this or disable default rb.velocity.x drag
        if (speed > 0)
        {
            if (currentMaxSpeed - speed > 10)
            {
                currentMaxSpeed = speed;
            }
        }
        else if (speed < 0)
        {
            //This one isn't actually tested so see if it works :D
            if (currentMaxSpeed - speed > -10)
            {
                currentMaxSpeed = -speed;
            }

        }

        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    // Debug function
    public void TestDamage()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeDamage(1);
        }
    }

    public void PlayerHealth()
    {
        if (health <= 0)
        {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
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
            currentMaxSpeed = goalMaxSpeed;

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
        if (rb.velocity.x > 1)
        {
            turned = false;
        }
        if (rb.velocity.x < -1)
        {
            turned = true;
        }

        if (!turned)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            this.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }

    bool IsGrounded()
    {
        RaycastHit2D hit = GroundrayCast();
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    public void MovementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || GetComponent<ThrowHook>().curHook.GetComponent<HookScript>().child.tag == "Anchored Grabable"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/2 + jumpVelocity);
        }

        // Speed up falling when going down or when releasing the jump button
        if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += (Vector2.up * Physics2D.gravity * (fallMultiplier - 1)) * Time.deltaTime;
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity += (Vector2.up * Physics2D.gravity * (gravityMultiplier - 1)) * Time.deltaTime;
        }

    }

    public void TakeDamage(float amount)
    {
        health = Mathf.Clamp(health -= amount, 0, maxHealth);
        FlashRed();
    }

    void FlashRed()
    {
        renderer.GetComponent<MeshRenderer>().material.color = Color.red;
        Invoke("ResetColor", flashTime);
    }

    void ResetColor()
    {
        renderer.GetComponent<MeshRenderer>().material.color = originalColor;
    }

}
