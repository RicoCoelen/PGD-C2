using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    public LayerMask Player;
    public GameObject playerPosition;

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
        // Enabel shooting cooldown on start
        timeToShoot = cooldownShoot;
    }
    // Update is called once per frame
    void Update()
    {
        TryShoot();

        // check if player is left or right in sights
        if (GetComponentInParent<EnemyMainScript>().currentTarget != null)
        {
            direction = (GetComponentInParent<EnemyMainScript>().currentTarget.transform.position - transform.position).normalized;
        }
       
    }

    private void TryShoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, shootRange, Player);
        //Debug.DrawLine(transform.position, transform.position + direction * shootRange);

        if (hit.collider == true)
        {
            // shooting attack
            if (timeToShoot <= 0)
            {
                isShooting = true;
                if (muzzleFlash != null)
                {
                    Instantiate(muzzleFlash, transform.position, transform.rotation);
                }
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
    }

    void OnDrawGizmosSelected()
    {
        // shoot raycast
        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + direction * shootRange);
    }
}
