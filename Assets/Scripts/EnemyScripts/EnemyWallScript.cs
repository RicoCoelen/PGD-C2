using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallScript : MonoBehaviour
{
    [Header("Wall Check Settings")]
    public GameObject parent;
    public LayerMask level;
    public LayerMask objects;
    public LayerMask enemies;
    public bool isWalled = false;
    public float wallCheckDistance;
    private float side;
    private bool facingRight;
    private Vector2 sideV;

    void Start()
    {
        side = transform.position.x + wallCheckDistance;
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, sideV, wallCheckDistance, level);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, sideV, wallCheckDistance, objects);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position, sideV, wallCheckDistance, enemies);

        // check if hit level
        if (hit.collider == true || hit2.collider == true || hit3.collider == true)
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
        Gizmos.DrawLine(transform.position, new Vector3(side, transform.position.y, transform.position.z));
    }
}
