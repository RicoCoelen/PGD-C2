using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // canvas or screen to hide and show buttons
    public GameObject deathPanel = null;
    public GameObject winPanel = null;
    public GameObject Player;

    // start
    public void Start()
    {
        Time.timeScale = 1;
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        
        Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (Player != null)
        {
            if (Player.GetComponent<PlayerScript>().lives <= 0)
            {
                GameOver();
            }
        }
    }

    // function to load next scene
    public void PlayNext()
    {
        Time.timeScale = 1;
        try
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        catch (Exception e)
        {
            Debug.Log(e + " Probably couldnt find the scene next scene");
        }
    }

    // function to load previous scene
    public void PlayPrevious()
    {
        Time.timeScale = 1;
        try 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        catch (Exception e)
        {
            Debug.Log(e + " Probably couldnt find the scene previous scene");
        }
    }

    // function to load back to menu
    public void BackMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    // function to quit game
    public void Quit()
    {
        Application.Quit();
    }

    // function to stop game and show death panel
    public void GameOver()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
            Time.timeScale = 0;
        }
        //else
        //{
        //    Quit();
        //}
    }

    public void GameWin()
    {
        if (winPanel != null)
        {
            Time.timeScale = 0;
            winPanel.SetActive(true);
        }
        else
        {
            PlayNext();
        }
    }

    // function to reload the current level
    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
