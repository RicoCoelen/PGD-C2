using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    // position
    public GameObject varChange;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        varChange.GetComponent<GrappleScript>().lastHit = collision.gameObject;
        varChange.GetComponent<GrappleScript>().lastHitPosition = collision.transform;
        Destroy(gameObject);
    }
}
