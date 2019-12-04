﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChainScript : MonoBehaviour
{
    public float maxDistance = 5;
    public DistanceJoint2D chainJoint;
    private PlayerScript playerScript;
    public LayerMask layer;

    Vector2 startingPoint;
    public bool isFlexible;

    public Vector2 direction;

    public float speed = 1;
    public float interfal = 2;

    public GameObject node;
    public GameObject player;
    public GameObject lastNode;

    bool done = false;
    bool stop = false;

    // Use this for initialization
    void Start()
    {
        // Initializes everything for old chain
        {
            // Sets references to other objects or scripts
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerScript>();


            // Calculates the direction from start- to endpoint
            startingPoint = player.transform.position;
            direction -= (Vector2)player.transform.position;
            direction.Normalize();
            direction *= maxDistance;
            direction += (Vector2)player.transform.position;

            // Keeps track of all the nodes in the chain
            lastNode = transform.gameObject;
        }

        // Initializes everything for the new chain
        {
            chainJoint = GetComponent<DistanceJoint2D>();

            chainJoint.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlexible)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
            chainJoint.distance = Vector2.Distance(player.transform.position, GetComponent<Rigidbody2D>().position);
            Debug.DrawLine(player.transform.position, GetComponent<Rigidbody2D>().position, Color.green);
            chainJoint.connectedBody = player.GetComponent<Rigidbody2D>();
            chainJoint.enabled = true;
        }




        // Old hook script
        {
            // Makes the hook move in the clicked direction and creating nodes for in the chains
            {
                // Moves the hook to
                if (!stop)
                    transform.position = Vector2.MoveTowards(transform.position, direction, speed);

                // Creates nodes while hook is traveling
                if (transform.position != (Vector3)direction && !stop)
                {
                    if (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
                    {
                        CreateNode();
                    }
                }
                // Creates remaning nodes and attaches player to chain
                else if (done == false)
                {
                    done = true;

                    while (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal + interfal / 2)
                    {
                        CreateNode();
                    }

                    lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                }
            }

            HitWall();
        }
    }

    /// <summary>
    /// Checks if the hook hit a wall and stops it from traveling
    /// </summary>
    void HitWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != player.GetComponent<Collider2D>() && hit.collider != null)
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + Vector2.up, Vector2.up, 0.1f);
            if (hit2.collider != player.GetComponent<Collider2D>() && hit2.collider != null)
            {
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                isFlexible = true;
            }
            else
                isFlexible = false;
            stop = true;
        }
    }

    /// <summary>
    /// Initializes nodes and connects it to the rest of the chain
    /// </summary>
    void CreateNode()
    {

        Vector2 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= interfal;
        pos2Create += (Vector2)lastNode.transform.position;

        GameObject go = Instantiate(node, pos2Create, Quaternion.identity);


        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

        lastNode = go;
    }
}