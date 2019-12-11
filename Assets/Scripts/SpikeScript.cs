using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public GameObject player;
    public float spikeDamage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // To come easy and hard mode for diffrent kind of On collisions

        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerScript>().TakeDamage(spikeDamage);
            //Debug.Log("collision");
        }
    }
}
