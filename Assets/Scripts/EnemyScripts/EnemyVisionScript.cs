using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionScript : MonoBehaviour
{
    public float searchTimer = 5;
    public float DetectionRange;
    public float lastDetectionTime;
    public float FOV = 90;
    private GameObject PlayerGO;
    public GameObject currentTarget;

    private void Start()
    {
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // get distance
        float dis = Vector3.Distance(transform.position, PlayerGO.transform.position);

        // check if in range
        if (dis < DetectionRange)
        {
            // get direction to player
            Vector3 rayDirection = PlayerGO.transform.position - transform.position;

            // raycast to player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude);
            if (hit)
            {
                if (hit.collider.tag == "Player")
                {
                    //Checks if the player is in the FOV
                    float angle = Vector2.Angle(rayDirection, Vector3.forward);
           
                    if (angle < FOV / 2)
                    {
                        //Debug.DrawLine(transform.position, hit.point);
                        GetComponentInParent<EnemyMainScript>().currentTarget = PlayerGO;
                        currentTarget = PlayerGO;
                        lastDetectionTime = Time.time;
                    }

                    
                }
            }
        }

        // make enemy forget after while
        if (currentTarget != null && Time.time - lastDetectionTime > searchTimer)
        {
            GetComponentInParent<EnemyMainScript>().currentTarget = null;
            currentTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Quaternion rot = Quaternion.AngleAxis(0, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis(90, Vector3.forward);

        if (GetComponentInParent<EnemyMainScript>().facingRight == true)
        {
            Gizmos.DrawLine(transform.position, transform.position + rot * Vector3.right);
            Gizmos.DrawLine(transform.position, transform.position + rot2 * Vector3.right);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position - rot * Vector2.right);
            Gizmos.DrawLine(transform.position, transform.position - rot2 * Vector2.right);
        }
    } 
}
