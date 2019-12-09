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
    public GameObject bulletPrefab;
    public float cooldownShoot;
    public float shootRange;
    public GameObject muzzleFlash;
    private float timeToShoot;
    private Vector2 direction;


    // Update is called once per frame
    void Update()
    {
        TryMelee();
        TryShoot();

        // check if player is left or right in sights
        if (GetComponentInParent<EnemyMainScript>().facingRight == true)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.left;
        }
    }

    private void TryShoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, shootRange, Player);

        if (hit.collider == true)
        {
            // melee attack
            if (timeToShoot <= 0)
            {
                isShooting = true;
                if (muzzleFlash != null)
                {
                    Instantiate(muzzleFlash, transform.position, transform.rotation);
                }
                GameObject temp = Instantiate(bulletPrefab, transform.position, transform.rotation);

                AudioManager.PlaySound("EnemyShot");

                // reset timer
                timeToShoot = cooldownShoot;
            }
            else
            {
                isShooting = false;
                // reset timer
                timeToShoot -= Time.deltaTime;
            }
        }
    }

    private void TryMelee()
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

        // shoot raycast
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x * shootRange, 0, 0));
    }
}
