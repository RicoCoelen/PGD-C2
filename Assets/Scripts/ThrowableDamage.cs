using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableDamage : MonoBehaviour
{
    protected Rigidbody2D rb;
    public float minForce = 1f;
    public float damageScalar = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.magnitude > minForce)
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    collision.gameObject.GetComponent<EnemyMainScript>().TakeDamage(1 * rb.velocity.magnitude);
                    Destroy(gameObject);
                    break;
            }
        }

    }
}
