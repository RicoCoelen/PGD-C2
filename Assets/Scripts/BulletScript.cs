using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public int minDamage;
    public int maxDamage;
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = direction * speed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                PlayerScript Player = collision.gameObject.GetComponent<PlayerScript>();
                if (Player != null)
                {
                    Player.TakeDamage(Random.Range(minDamage, maxDamage));
                }
                Destroy(gameObject); 
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

}
