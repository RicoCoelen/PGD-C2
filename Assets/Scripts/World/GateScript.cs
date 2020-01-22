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

    public GameObject textKilledOutOf;
    public GameObject textTime;

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
            // calculate how many are killed and show on menu
            aantalEnemiesEnd = Enemies.Length;
            killedEnemies = aantalEnemies - aantalEnemiesEnd;
            textKilledOutOf.GetComponent<Text>().text = ("You killed: " + killedEnemies.ToString()+ " from the " + aantalEnemies.ToString() + " possible kills");

            // Getting the info for the timer and par timer.
            int currentParTime = PlayerPrefs.GetInt(Timer.GetComponent<TimeScript>().levelName);

            // Get the current time as best time if the best time is empty
            if (currentParTime < 2)
            {
                currentParTime = (int)Timer.GetComponent<TimeScript>().time;
                PlayerPrefs.SetInt(Timer.GetComponent<TimeScript>().levelName, currentParTime);
            }

            if (currentParTime > Timer.GetComponent<TimeScript>().time)
            {
                PlayerPrefs.SetInt(Timer.GetComponent<TimeScript>().levelName, (int)Timer.GetComponent<TimeScript>().time);
                currentParTime = (int)Timer.GetComponent<TimeScript>().time;
            }

            int parMinutes = currentParTime / 60;
            int parSeconds = currentParTime % 60;

            // Changing the timer text.
            textTime.GetComponent<Text>().text = "You finished in: " + Timer.GetComponent<TimeScript>().minutes + ":" + Timer.GetComponent<TimeScript>().seconds + " The record time is: " + parMinutes + ":" + parSeconds;

            // show end menu and stop time
            Manager.GetComponent<GameManager>().GameWin();
        }
    }
}
