using System.Collections;
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
    public float chasingSpeed = 10f;
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
        // always start idle
        cState = State.IDLE;
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
        // check velocity and let enemy stay left after walking
        if (rb.velocity.x > 0 && !facingRight || rb.velocity.x < 0 && facingRight)
        {
            facingRight = !facingRight;
        }

        // rotate gameobject
        if (facingRight == true)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void EnemyIdle()
    {
        // todo but just stand still
        cState = State.PATROLLING;
    }

    void EnemyPatrolling()
    {
        if (currentTarget != null)
        {
            cState = State.CHASING;
        }
        else
        {
            if (isGrounded == false || isWalled == true)
            {
                movementSpeed *= -1;
                facingRight = !facingRight;
            }
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
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
            // move current rigidbody to tracked player
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            direction.y = 0;

            if (currentTarget.transform.position.x > transform.position.x)
            {
                facingRight = true;
                
                if (isGrounded == true)
                {
                    chasingSpeed = Mathf.Abs(chasingSpeed);
                    rb.velocity = new Vector2(direction.x * chasingSpeed * Time.deltaTime, rb.velocity.y);
                }
            }
            else
            {
                facingRight = false;
                
                if (isGrounded == true)
                {
                    chasingSpeed = -Mathf.Abs(chasingSpeed);
                    rb.velocity = new Vector2(-direction.x * chasingSpeed * Time.deltaTime, rb.velocity.y);
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

        // if no health die
        if (health <= 0f)
        {
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
