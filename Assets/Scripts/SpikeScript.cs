using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public GameObject player;
    public float spikeDamage = 1;
    private float lastUpdate;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastUpdate >= 1f)
            {
                player.GetComponent<PlayerScript>().TakeDamage(spikeDamage);
                lastUpdate = Time.time;

            }
        }
    }
}
