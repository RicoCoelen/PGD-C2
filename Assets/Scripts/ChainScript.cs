﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChainScript : MonoBehaviour
{
    public Vector2 direction;

    public float speed = 1;

    public float interfal = 2;

    public GameObject node;

    public GameObject player;

    public GameObject lastNode;

    bool done = false;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        lastNode = transform.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = Vector2.MoveTowards(transform.position, direction, speed);

        if ((Vector2)transform.position != direction)
        {

            if (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
            {

                CreateNode();

            }


        }
        else if (done == false)
        {

            done = true;


            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
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
        
    }

}