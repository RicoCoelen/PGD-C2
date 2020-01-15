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
    public float forceDamageLimit = 10f;
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

    // Stand idle and attack the enemy when in sight
    void EnemyIdle()
    {
        if (currentTarget != null)
        {
            animator.SetBool("isAiming", true);
        }
        else
        {
            animator.SetBool("isAiming", false);
        }
    }

    // Move left and right, turning around when reaching the end of a platform or when walking into a wall.
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

    // Stand still and aim at the enemy, turn around based on the enemy position and go back to patrolling after losing target
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Grabable":
                // if thrown object hits enemy die, instantiate broken prefab
                if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > forceDamageLimit)
                {
                    TakeDamage(9999);
                    collision.gameObject.GetComponent<BreakableScript>().Break();
                }
                break;
        }
    }
}
