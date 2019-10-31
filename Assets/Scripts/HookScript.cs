﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookScript : MonoBehaviour
{
    public Transform child;
    public GameObject hook;
    GameObject player;
    public LayerMask grabable;
    public float throwForce;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {

        if(child != null)
            child.position = transform.position;

        if(child != null && Input.GetMouseButtonDown(0))
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            child.parent = null;
            direction -= (Vector2)transform.position;
            direction.Normalize();
            child.GetComponent<Rigidbody2D>().velocity += direction * throwForce;

            Destroy(hook);

            player.GetComponent<ThrowHook>().active = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            collision.gameObject.transform.parent = hook.transform;
            child = collision.gameObject.transform;

            Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
        }
    }
}
