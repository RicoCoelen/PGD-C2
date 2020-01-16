using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallScript : MonoBehaviour
{
    [Header("Wall Check Settings")]
    public GameObject parent;
    public LayerMask obstacles;
    public bool isWalled = false;
    public float wallCheckDistance;
    private float side;
    private bool facingRight;
    private Vector2 sideV;
    EnemyMainScript enemyMainScript;

    void Start()
    {
        side = transform.position.x + wallCheckDistance;
        enemyMainScript = GetComponentInParent<EnemyMainScript>();
    }

    void FixedUpdate()
    {
        if (parent.GetComponent<EnemyMainScript>().facingRight == true)
        {
            side = transform.position.x + wallCheckDistance;
            sideV = Vector2.right;
        }
        else
        {
            side = transform.position.x - wallCheckDistance;
            sideV = Vector2.left;
        }

        // raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, sideV, wallCheckDistance, obstacles);

        // check if hit level
        if (hit.collider == true)
        {
            isWalled = true;
            parent.GetComponent<EnemyMainScript>().isWalled = true;
        }
        else
        {
            isWalled = false;
            parent.GetComponent<EnemyMainScript>().isWalled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + wallCheckDistance, transform.position.y, transform.position.z));
    }
}
