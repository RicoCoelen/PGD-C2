using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // attack cooldowntimer
    private float timeToAttack;
    public float cooldownAttack;

    // attack positon / range
    public Transform attackPos;
    public float attackRange;

    // enemy stats
    public float movementSpeed = 10;
    private bool facingRight = true;
    private float health = 100f;
    public float fleeLimit = 20f;
    private float direction;

    // children for attack, wall and ground
    public GameObject AttackCheck;
    public GameObject WallCheck;
    public GameObject GroundCheck;

    // checks for movement
    public bool isGrounded = false;
    public bool isWalled = false;
    public float wallDistance;
    public float groundDistance;
    public float side;

    // player 
    public LayerMask Player;
    public LayerMask Level;

    // huidige status
    private State cState;

    // current enemy target
    public GameObject currentTarget = null;

    // enemy rigidbody
    Rigidbody2D rb;

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
        // face right
        side = WallCheck.transform.position.x + wallDistance;
        // always start idle
        cState = State.IDLE;
    }

    private void FixedUpdate()
    {
        // flip gameobject to correct side
        FlipCorrection();
        // do checks
        isGrounded = groundCheck();
        isWalled = wallCheck();
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

            case State.FLEEING:
                EnemyFleeing();
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

    void EnemyFleeing()
    {

        if (currentTarget == null || fleeLimit > health)
        {
            cState = State.PATROLLING;
        }
        else
        {
            // move current rigidbody to tracked player
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            rb.MovePosition(transform.position + -direction * movementSpeed * Time.deltaTime);
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
            rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);

            // check for player to damage
            CheckDamage();

            if (fleeLimit > health)
            {
                cState = State.FLEEING;
            }
        }
    }

    public bool groundCheck()
    {
        // raycast to ground
        RaycastHit2D hit = Physics2D.Raycast(GroundCheck.transform.position, Vector2.down, groundDistance);
        
        if (hit.collider == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool wallCheck()
    {
        Vector2 sideV;

        if (facingRight == true)
        {
            side = WallCheck.transform.position.x + wallDistance;
            sideV = Vector2.right;
        }
        else
        {
            side = WallCheck.transform.position.x - wallDistance;
            sideV = Vector2.left;
        }

        RaycastHit2D hit = Physics2D.Raycast(GroundCheck.transform.position, sideV, wallDistance, Level);
        if (hit.collider == true)
        {
            return true;
        }
        else
        {
            return false;
        }   
    }

    void CheckDamage()
    {
        // melee attack
        if (timeToAttack <= 0)
        {
            // check if something enters collision area
            Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, Player);

            // if not empty
            if (playerToDamage.Length > 0)
            {
                // do something with collision like access damage script from here
            }

            // reset timer
            timeToAttack = cooldownAttack;
        }
        else
        {
            // reset timer
            timeToAttack -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        // attack zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

        // ground
        Gizmos.DrawLine(GroundCheck.transform.position, new Vector3(GroundCheck.transform.position.x, GroundCheck.transform.position.y - groundDistance, transform.position.z));

        // wall
        Gizmos.DrawLine(WallCheck.transform.position, new Vector3(side, WallCheck.transform.position.y, WallCheck.transform.position.z));
    }
}
