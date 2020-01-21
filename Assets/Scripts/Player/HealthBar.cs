using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] GameObject player;
    [SerializeField] GameObject text;
    PlayerScript playerScript;
    Image healthImage;
    Text healthText;

    public Image[] hearts;
    public int heartAmount;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private int health;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Get the components
        playerScript = player.GetComponent<PlayerScript>();
        healthText = text.GetComponent<Text>();
   
        heartAmount = (int)playerScript.lives;

    }

    // Update is called once per frame
    void Update()
    {
        health = (int)playerScript.lives;
        //healthText.text = "Lives: " + health;

        if (health > heartAmount)
        {
            heartAmount = health;
        }
    
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < heartAmount)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}
