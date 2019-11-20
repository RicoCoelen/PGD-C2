using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundScript : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public GameObject parent;
    public LayerMask Level;
    public bool isGrounded = false;
    public float groundCheckDistance;

    void FixedUpdate()
    {
        // raycast to ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, Level);

        if (hit.collider == true)
        {
            isGrounded = true;
            parent.GetComponent<EnemyMainScript>().isGrounded = true;
        }
        else
        {
            isGrounded = false;
            parent.GetComponent<EnemyMainScript>().isGrounded = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance, transform.position.z));
    }
}
