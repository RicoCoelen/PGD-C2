using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookScript : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject hook;
    public Transform child;

    [Header("Attack")]
    public float minDamage;
    public float maxDamage;

    [Header("Layer")]
    public LayerMask grabable;

    [Header("Input")]
    public int inputButton;

    [Header("Throw")]
    public float throwForce;


    [System.NonSerialized] public bool active;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (child != null)
        {
           
            child.position = transform.position;
        }

        if (child != null && Input.GetMouseButtonDown(inputButton))
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            child.parent = null;
            direction -= (Vector2)transform.position;
            throwForce = Vector3.Magnitude(direction) * 3;
            direction.Normalize();
            child.GetComponent<Rigidbody2D>().velocity += direction * throwForce;

            Destroy(hook);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Grabable":               
                collision.gameObject.transform.parent = hook.transform;
                child = collision.gameObject.transform;
                Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                Physics2D.IgnoreCollision(child.GetComponent<Collider2D>(), player.GetComponent<Collider2D>());
                break;

            case "Anchored Grabable":
                hook.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                child = collision.gameObject.transform;
                hook.transform.position = collision.gameObject.transform.position;
                break;

            case "Enemy":
                collision.gameObject.GetComponent<EnemyMainScript>().TakeDamage(Random.Range(minDamage, maxDamage));
                Destroy(hook);
                active = false;
                break;

            case "Breakable Wall":
                collision.gameObject.GetComponent<BreakableWall>().BreakWall();
                Destroy(hook);
                break;

            
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Switch":
                collision.gameObject.GetComponent<ButtonSwitches>().switchState();
                Destroy(hook);
                break;

        }
    }
}
