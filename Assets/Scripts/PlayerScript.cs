using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    [Header("MoveSpeed settings")]
    float currentMaxSpeed;
    float currentAcceleration;
    float currentReactionMultiplier;
    public float maxSpeed = 80f;
    public float sprintMaxSpeed = 100f;
    public float maxAcceleration = 40f;
    public float sprintAcceleration = 80f;
    public float drag = 40f;
    public float reactionMultiplier = 0.5f;
    public float sprintReactionMultiplier = 0.25f;
    float speed = 0f;
    bool moving = false;

    public float moveSpeed = 60f;

    bool turned = false;


    //public SpriteRenderer render;

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


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalColor = renderer.GetComponent<MeshRenderer>().material.color;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        Crouch();
        MovementJump();
        PlayerTurn();
        // CheckFlipToMouse();
        PlayerHealth();
        // debug
        TestDamage();
        //Debug.Log(health);
    }

  
    private void Crouch()
    {
        //if (crouching)
        //{

        float hillMultiplier = 2f;

        //TODO: when crouching, slide down hills.
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, rayLength, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log(hit.normal.x + "___ _ _ _: " + speed + "__: " + currentMaxSpeed);
            if (hit.normal.x < 1 && hit.normal.x > -1 && hit.normal.x != 0)
            {
                if (hit.normal.x > 0 && rb.velocity.x > 0)
                { 
                    sliding = true;
                    speed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                }

                if (hit.normal.x < 0 & rb.velocity.x < 0)
                {
                    sliding = true;
                    speed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                }

                if ((speed == currentMaxSpeed || speed == -currentMaxSpeed || currentMaxSpeed == 0) && sliding)
                {
                    if (speed > 0)
                        currentMaxSpeed += (hit.normal.x * hillMultiplier) * Time.deltaTime;
                    if (speed < 0)
                        currentMaxSpeed -= (hit.normal.x * hillMultiplier) * Time.deltaTime;
                }
            }
            else
            {
                sliding = false;
            }
        }
        else
        {
            sliding = false;
        }

        //}
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
    public void Momentum(float maximumSpeed)
    {
        // if the player is on the ground
        if (currentMaxSpeed > maximumSpeed && IsGrounded())
        {
            currentMaxSpeed -= (drag * 0.2f) * Time.deltaTime;
        }
        else if (currentMaxSpeed > maximumSpeed && !IsGrounded())
        {
            currentMaxSpeed -= (drag * 0.05f) * Time.deltaTime;
        }
    }
    public void PlayerMovement()
    {
        // Leftshift and rightshift don't work in unity like normal keys because ?????
        if (Input.GetButton("Run"))
        {
            sprinting = true;
        } else
        {
            sprinting = false;
        }

        // While holding the sprint button the maxSpeed, acceleration and reaction multiplier change
        if (sprinting)
        {
            // if the previous max speed is higher than the running high speed, run momentum
            if (currentMaxSpeed > sprintMaxSpeed)
            {
                Momentum(sprintMaxSpeed);
            }
            else
            {
                currentMaxSpeed = sprintMaxSpeed;
                currentAcceleration = sprintAcceleration;
                currentReactionMultiplier = sprintReactionMultiplier;
            }

        }
        else
        {
            // if the previous max speed is higher than the standard high speed, run momentum
            if (currentMaxSpeed > maxSpeed)
            {
                Momentum(maxSpeed);
                
            } 
            else
            {
                currentMaxSpeed = maxSpeed;
                currentAcceleration = maxAcceleration;
                currentReactionMultiplier = reactionMultiplier;
            }


        }


        if (GetComponent<ThrowHook>().active)
        {
            float playerMovement = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(playerMovement * Time.deltaTime * moveSpeed + rb.velocity.x, rb.velocity.y);
        }
        else
        {
            if (rb.velocity.x == 0)
                speed = 0;
            if (rb.velocity.x > speed || rb.velocity.x < -speed)
                 speed = rb.velocity.x;

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

                speed = Mathf.Min(speed, currentMaxSpeed);
                moving = true;
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

                speed = Mathf.Max(speed, -currentMaxSpeed);
                moving = true;
            }
            else
            {
                moving = false;
            }

            if (!sliding)
            {
                // if no movement keys are pressed lower or increase speed by the drag value until it goes back to 0
                if (!moving && speed > 0)
                {
                    speed -= (drag * Time.deltaTime);
                    if (speed < 0.1)
                        speed = 0;
                }
                else if (!moving && speed < 0)
                {
                    speed += (drag * Time.deltaTime);
                    if (speed > 0.1)
                        speed = 0;
                }
            }

            rb.velocity = new Vector2(speed, rb.velocity.y);
        }

    }

    void PlayerTurn()
    {
        // Turn the player around if they are moving in one direction above a certain speed
        if (rb.velocity.x > 1)
        {
            turned = false;

            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        if (rb.velocity.x < -1)
        {
            turned = true;
            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().flipX = true;
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
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, rayLength, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    public void MovementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || GetComponent<ThrowHook>().active))
        //GetComponent<ThrowHook>().curHook.GetComponent<ChainScript>().lastNode.GetComponent<HingeJoint2D>().connectedBody == GetComponent<Rigidbody2D>()))
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
