﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChainScript : MonoBehaviour
{
    // Max chain length
    public float maxDistance = 5;
    Vector2 startingPoint;
    public bool isFlexible;

    public Vector2 direction;

    public float speed = 1;
    public float interfal = 2;

    public GameObject node;
    public GameObject player;
    public GameObject chainEnd;
    public GameObject lastNode;
    
    public LayerMask grabables;

    bool done = false;
    bool stop = false;

    // Line render
    LineRenderer lr;
    List<GameObject> Nodes = new List<GameObject>();
    int vertexCount = 2;

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();

        player = GameObject.FindGameObjectWithTag("Player");

        startingPoint = player.transform.position;
        direction -= (Vector2)player.transform.position;
        direction.Normalize();
        direction *= maxDistance;
        direction += (Vector2)player.transform.position;

        lastNode = transform.gameObject;
        Nodes.Add(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the hook move in the clicked direction and creating nodes for in the chains
        {
            if(!stop)
                transform.position = Vector2.MoveTowards(transform.position, direction, speed);

            if (transform.position != (Vector3)direction && !stop)
            {
                if (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
                {
                    CreateNode();
                }
            }
            else if (done == false)
            {
                done = true;

                while (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
                {
                    CreateNode();
                }

                lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            }
        }

        HitGrabable();
    }

    void HitGrabable()
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
            }else
                isFlexible = false;
            stop = true;
        }
    }


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

        Nodes.Add(lastNode);
        vertexCount++;
    }

}