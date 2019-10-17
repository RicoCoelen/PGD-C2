using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    Rigidbody2D rb;

    public float jumpVelocity = 25f;
    public float fallMultiplier = 7.5f;
    public float gravityMultiplier = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        movementJump();
    }


    void movementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.up * jumpVelocity;
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
