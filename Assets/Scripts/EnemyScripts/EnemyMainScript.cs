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
    public float movementSpeed = 10;
    public float health = 100f;
    public float fleeLimit = 20f;

    [Header("Behaviour")]
    public State cState;

    [Header("Extra vars")]
    public LayerMask Player;
    public GameObject currentTarget = null;
    public GameObject PlayerGO;

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
    }

    private void FixedUpdate()
    {
        // flip gameobject to correct side
        FlipCorrection();
    }

    // Update is called once per frame
    void Update()
    {
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
                if (isGrounded == true)
                {
                    rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
                }
                facingRight = true;
                movementSpeed = Mathf.Abs(movementSpeed);
            }
            else
            {
                if (isGrounded == true)
                {
                    rb.MovePosition(transform.position + -direction * movementSpeed * Time.deltaTime);
                }
                facingRight = false;
                movementSpeed = -Mathf.Abs(movementSpeed);
            }
        }
    }
}
