using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public float forceDamageLimit = 1f;
    public GameObject brokenBits;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Grabable":
                if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > forceDamageLimit)
                {
                    BreakWall();
                    collision.gameObject.GetComponent<BreakableScript>().Break();
                }
                break;
        }
    }

    public void BreakWall()
    {
        GameObject brokenBox = Instantiate(brokenBits, transform.position, Quaternion.identity);
        AudioManager.PlaySound("BreakWall");
        Destroy(this.gameObject);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
