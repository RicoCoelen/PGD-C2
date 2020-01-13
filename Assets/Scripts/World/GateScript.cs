using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GateScript : MonoBehaviour
{
    private GameObject[] Enemies;
    private int aantalEnemies;
    private int aantalEnemiesEnd;
    private int killedEnemies;
    private GameObject Timer;

    public GameObject Manager;

    public GameObject textKilled;
    public GameObject textKilledOutOf;
    public GameObject textCurrentTime;
    public GameObject textParTime;

    public void Start()
    {
        // get start variables, how many enemies are in the level
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        aantalEnemies = Enemies.Length;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && Input.GetKey(KeyCode.W))
        {
            // find timer and enemy if exist
            Timer = GameObject.Find("Timer");
            Enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // het howmany enemies are left and show it on menu
            aantalEnemiesEnd = Enemies.Length;
            textKilledOutOf.GetComponent<Text>().text = aantalEnemies.ToString();

            // calculate how many are killed and show on menu
            killedEnemies = aantalEnemies - aantalEnemiesEnd;
            textKilled.GetComponent<Text>().text = killedEnemies.ToString();

            // change the text of timer in end menu
            textCurrentTime.GetComponent<Text>().text = Timer.GetComponent<TimeScript>().minutes + ":" + Timer.GetComponent<TimeScript>().seconds;
            textParTime.GetComponent<Text>().text = Timer.GetComponent<TimeScript>().currentParTime;

            // show end menu and stop time
            Manager.GetComponent<GameManager>().GameWin();
        }
    }
}
