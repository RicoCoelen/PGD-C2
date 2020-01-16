using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    PlayerScript playerScript;
    public bool currentActive = false;
    SpriteRenderer spriteRenderer;
    ParticleSystem particleSystem;

    public Sprite activated;
    public Sprite disabled;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentActive)
        {
            spriteRenderer.sprite = activated;
        }
        else
        {
            spriteRenderer.sprite = disabled;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!currentActive)
        {
            playerScript.CheckpointReached(this);
            currentActive = true;
            particleSystem.Play();
        }
    }
}
