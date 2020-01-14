﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMainScript : MonoBehaviour
{
    [Header("Private Vars")]
    private Rigidbody2D rb;
    public bool facingRight = true;
    public bool isGrounded = false;
    public bool isWalled = false;

    [Header("Enemy Stats")]
    public float movementSpeed = 10f;
    private float currentMoveSpeed;
    public float health = 100f;
    public float fleeLimit = 20f;
    public float knockBackForce = 10f;
    public float flashTime = 1f;
    private Color originalColor;

    [Header("Behaviour")]
    public State cState;

    [Header("Extra vars")]
    public LayerMask Player;
    public GameObject currentTarget = null;
    private GameObject PlayerGO;

    public Animator animator;
    public GameObject visorLight;

    // State Types
    public enum State
    {
        IDLE = 0,
        PATROLLING = 1,
        CHASING = 2,
        FLEEING = 3
    }

    // Start is called before the first frame update
    void Start()
    {
        // initiate rigidbody
        rb = GetComponent<Rigidbody2D>();
        originalColor = Color.white;
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        FlipCorrection();

        // check state and handle accordingly
        switch (cState)
        {
            case State.IDLE:
                EnemyIdle();
                break;

            case State.PATROLLING:
                EnemyPatrolling();
                break;

            case State.CHASING:
                EnemyChasing();
                break;

            default:
                cState = State.IDLE;
                break;
        }
    }

    void FlipCorrection()
    {
        // rotate gameobject
        if (facingRight == true)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);          
            currentMoveSpeed = movementSpeed;
            if (currentTarget == null)
            {
                visorLight.transform.rotation = Quaternion.Euler(0, 0, -90);
            }

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);        
            currentMoveSpeed = -movementSpeed;
            if (currentTarget == null)
            {
                visorLight.transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
    }

    void EnemyIdle()
    {
        // todo but just stand still
        //cState = State.PATROLLING;
    }

    void EnemyPatrolling()
    {
        if (currentTarget != null)
        {
            cState = State.CHASING;        
        }
        else
        {
            animator.SetBool("isAiming", false);

            if (isGrounded == false || isWalled == true)
            {
                facingRight = !facingRight;
                FlipCorrection();
            }
            rb.velocity = new Vector2(currentMoveSpeed, rb.velocity.y);
        }
    }

    void EnemyChasing()
    {
        if (currentTarget == null)
        {
            cState = State.PATROLLING;
        }
        else
        {
            animator.SetBool("isAiming", true);

            // Stand still and aim at the player
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Check if the player can still be seen
            if (GetComponentInChildren<EnemyAttackScript>().canSeeEnemy)
            {
                // Get direction to the player
                if (currentTarget.transform.position.x < transform.position.x)
                {
                    facingRight = false;
                }
                else
                {
                    facingRight = true;
                }
            }

        }

    }

    public void TakeDamage(float amount)
    {
        // add knockback
        Vector3 moveDirection = transform.position - PlayerGO.transform.position;
        moveDirection.y = 0;
        rb.AddForce(moveDirection.normalized * knockBackForce);

        // remove health
        health -= amount;

        // flash red
        FlashRed();

        AudioManager.PlaySound("EnemyHit");

        // if no health die
        if (health <= 0f)
        {
            AudioManager.PlaySound("Death");
            Destroy(gameObject);
        }
    }

    private void FlashRed()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
        Invoke("ResetColor", flashTime);
    }

    private void ResetColor()
    {
        GetComponentInChildren<SpriteRenderer>().color = originalColor;
    }

}
