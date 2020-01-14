using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    public LayerMask Player;
    public GameObject playerPosition;
    public GameObject visorLight;
    public bool canSeeEnemy;

    [Header("Ranged Attack")]
    public bool isShooting = false;
    public GameObject bulletPrefab;
    public float cooldownShoot;
    public float shootRange;
    public GameObject muzzleFlash;
    private float timeToShoot;
    private Vector3 direction;

    
    private void Start()
    {
        // Enable shooting cooldown on start
        resetTimeToShoot();
        playerPosition = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
        // check if player is left or right in sights
        if (GetComponentInParent<EnemyMainScript>().currentTarget != null)
        {
            direction = (playerPosition.transform.position - transform.position).normalized;
            TryShoot();

        } else
        {
            resetTimeToShoot();
        }
    }

    private void TryShoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, shootRange, Player);
        //Debug.DrawLine(transform.position, transform.position + direction * shootRange);

        if (hit.collider == true)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                canSeeEnemy = true;
                // Get the angle to the player and rotate the visor light towards them
                Vector3 difference = playerPosition.transform.position - transform.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                rotationZ -= 90;
                visorLight.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

                // shooting attack
                if (timeToShoot <= 0)
                {
                    isShooting = true;
                    //if (muzzleFlash != null)
                    //{
                    //    Instantiate(muzzleFlash, transform.position, transform.rotation);
                    //}
                    GameObject temp = Instantiate(bulletPrefab, transform.position, transform.rotation);
                    temp.gameObject.GetComponent<BulletScript>().direction = direction;

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
            else
            {
                canSeeEnemy = false;
            }        
        }
    }
    public void resetTimeToShoot()
    {
        timeToShoot = cooldownShoot;
    }

    void OnDrawGizmosSelected()
    {
        // shoot raycast
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + direction * shootRange);
    }
}
