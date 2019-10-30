using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GateScript : MonoBehaviour
{

    public string nextLevel = "NextLevelTest";

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            SceneManager.LoadScene(nextLevel);
        }


    }
}
