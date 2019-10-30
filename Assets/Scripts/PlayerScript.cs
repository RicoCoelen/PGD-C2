﻿using System.Collections;
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


    void MovementJump()
    {
        // Initial jump
        if (Input.GetButtonDown("Jump"))
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
