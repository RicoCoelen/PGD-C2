using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;

    public SpriteRenderer render;

    Rigidbody2D rb;

    public float jumpVelocity = 25f;
    public float fallMultiplier = 7.5f;
    public float gravityMultiplier = 0.01f;
    public float rayLength = 1;
    public LayerMask groundLayer;

    public float maxHealth = 10f;
    public float health = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        CheckFlipToMouse();
        PlayerHealth();

        // debug
        TestDamage();
        //Debug.Log(health);
    }

    public void TestDamage()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            health--;
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
        float playerMovement = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(playerMovement * Time.deltaTime * moveSpeed + rb.velocity.x, rb.velocity.y);
    }

    void FixedUpdate()
    {
        MovementJump();
    }

    void CheckFlipToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        float yRotation;
        if (mousePosition.x > transform.position.x)
        {
            yRotation = 0;
            render.flipX = false;
        }
        else
        {
            yRotation = 180;
            render.flipX = true;
        }
        transform.localRotation = Quaternion.Euler(0, yRotation, 0);
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
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        
        // Speed up falling when going down or when releasing the jump button
        if (rb.velocity.y < 0 || (rb.velocity.y > 0 && !Input.GetButton("Jump")))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else
        {
           rb.velocity += Vector2.up * Physics2D.gravity * (gravityMultiplier - 1) * Time.deltaTime;
        }
        
    }
}
