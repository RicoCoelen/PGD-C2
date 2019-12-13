using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public Transform position1;
    public Transform position2;
    public Transform startingPosition;

    public float speed = 5;

    Vector2 nextPos;

    // Start is called before the first frame update
    void Start()
    {
        nextPos = startingPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the position is the same without the z axis
        if (transform.position.x == position1.position.x && transform.position.y == position1.position.y)
        {
            nextPos = position2.position;
        }
        else if (transform.position.x == position2.position.x && transform.position.y == position2.position.y)
        {
            nextPos = position1.position;
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(position1.position, position2.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }
}
