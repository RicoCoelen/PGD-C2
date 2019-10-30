using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] GameObject player;
    PlayerScript playerScript;

    private void Awake()
    {
        // Get the playerscript
        playerScript = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the fillamount of the healthbar based on the players current and maxhealth
        health.GetComponent<Image>().fillAmount = playerScript.health / playerScript.maxHealth;
    }
}
