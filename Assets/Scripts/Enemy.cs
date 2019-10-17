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

    // player 
    public LayerMask Player;

    // huidige status
    private State cState;

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
        cState = State.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (cState)
        {
            case State.IDLE:

                break;

            case State.PATROLLING:

                break;

            case State.CHASING:
                // check for a player to damage
                checkDamage();
                break;

            case State.FLEEING:

                break;

            default:
                // do nothing
                break;
        }
    }

    void checkDamage()
    {
        // melee attack
        if (timeToAttack <= 0)
        {
            // check if something enters collision area
            Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, Player);

            // if not empty
            if (playerToDamage.Length > 0)
            {
                // do something with collision
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
