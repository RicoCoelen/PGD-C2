using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] GameObject player;
    [SerializeField] GameObject text;
    PlayerScript playerScript;
    Image healthImage;
    Text healthText;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // Get the components
        playerScript = player.GetComponent<PlayerScript>();
        healthImage = health.GetComponent<Image>();
        healthText = text.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        // Change the fillamount of the healthbar based on the players current and maxhealth
        healthImage.fillAmount = playerScript.lives / playerScript.maxLives;
        healthText.text = "Lives: " + playerScript.lives;
    }
}
