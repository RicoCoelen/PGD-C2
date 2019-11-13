using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    [Header("Melee Attack")]
    public LayerMask Player;
    public bool isStabbing = false;
    public float attackRange;
    public float cooldownAttack;
    public float meleeDamage;
    private float timeToAttack;

    [Header("Ranged Attack")]
    public bool isShooting = false;

    // Update is called once per frame
    void Update()
    {
        // melee attack
        if (timeToAttack <= 0)
        {
            // check if something enters collision area
            Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(transform.position, attackRange, Player);

            // if not empty
            if (playerToDamage.Length > 0)
            {
                // activate animation
                isStabbing = true;
                // loop trough attacked game objects
                for (int i = 0; i < playerToDamage.Length; i++)
                {
                    if (playerToDamage[i].gameObject.CompareTag("Player"))
                    {
                        playerToDamage[i].GetComponent<PlayerScript>().TakeDamage(meleeDamage);
                    }
                }
            }
            else
            {
                // stop animation
                isStabbing = false;
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
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
