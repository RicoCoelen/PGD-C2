using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float maxSpeed = 80f;
    public float acceleration = 40f;
    public float drag = 40f;
    public float reactionMultiplier = 0.5f;
    float speed = 0f;
    bool moving = false;

    public float moveSpeed = 60f;
    

    //public SpriteRenderer render;

    Rigidbody2D rb;

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

    public KeyCode right = KeyCode.RightArrow;
    public KeyCode left = KeyCode.LeftArrow;
    public KeyCode altRight = KeyCode.D;
    public KeyCode altLeft = KeyCode.A;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalColor = renderer.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        MovementJump();
       // CheckFlipToMouse();
        PlayerHealth();

        // debug
        TestDamage();
        //Debug.Log(health);
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

    public void PlayerMovement()
    {

        if (GetComponent<ThrowHook>().active)
        {
            float playerMovement = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(playerMovement * Time.deltaTime * moveSpeed + rb.velocity.x, rb.velocity.y);
        }
        else
        { 

            // Add acceleration to speed if the keys for going right or left are pressed.
            if (Input.GetKey(right) || Input.GetKey(altRight))
            {
                if (speed < 0)
                {
                    speed += ((acceleration + acceleration * reactionMultiplier) * Time.deltaTime);
                }
                else
                {
                    speed += (acceleration * Time.deltaTime);
                }

                speed = Mathf.Min(speed, maxSpeed);
                moving = true;
            }
            else if (Input.GetKey(left) || Input.GetKey(altLeft))
            {
                if (speed > 0)
                {
                    speed -= ((acceleration + acceleration * reactionMultiplier) * Time.deltaTime);
                }
                else
                {
                    speed -= (acceleration * Time.deltaTime);
                }

                speed = Mathf.Max(speed, -maxSpeed);
                moving = true;
            }
            else
            {
                moving = false;
            }

            // if no movement keys are pressed lower or increase speed by the drag value until it goes back to 0
            if (!moving && speed > 0)
            {
                speed -= (drag * Time.deltaTime);
            }
            else if (!moving && speed < 0)
            {
                speed += (drag * Time.deltaTime);
            }

            rb.velocity = new Vector2(speed, rb.velocity.y);
        }

        //Debug.Log(speed);
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

    void MovementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || GetComponent<ThrowHook>().active))
            //GetComponent<ThrowHook>().curHook.GetComponent<ChainScript>().lastNode.GetComponent<HingeJoint2D>().connectedBody == GetComponent<Rigidbody2D>()))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
        
        
        // Speed up falling when going down or when releasing the jump button
        if (rb.velocity.y < 0 || (rb.velocity.y > 0 && !Input.GetButtonDown("Jump")))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else
        {
           rb.velocity += Vector2.up * Physics2D.gravity * (gravityMultiplier - 1) * Time.deltaTime;
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
