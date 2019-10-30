using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameObject health;

    public GameObject player;
    PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        health = GameObject.Find("Health");
    }

    private void Awake()
    {
        // Get the playerscript
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the fillamount of the healthbar based on the players current and maxhealth
        health.GetComponent<Image>().fillAmount = playerScript.health / playerScript.maxHealth;
    }
}
