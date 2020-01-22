using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    float startTime;
    public float time;
    public string minutes;
    public string seconds;
    private bool runonce = true;

    [SerializeField] Text parTime;
    public int currentParTime = 500;
    Text text;

    public string levelName;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        if (PlayerPrefs.GetInt(levelName) > 2)
        {
            currentParTime = PlayerPrefs.GetInt(levelName);
        }


        // Only show the Par time UI if there is a par time set
        int parMinutes = currentParTime / 60;
        int parSeconds = currentParTime % 60;

        string min = parMinutes.ToString();
        string sec = "";

        if (parSeconds < 10)
        {
            sec = "0" + parSeconds.ToString();
        } 
        else
        {
            sec = parSeconds.ToString();
        }

        parTime.GetComponent<Text>().text = "Best Time: " + min + ":" + sec;
        text = GetComponent<Text>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runonce == true)
        {
            startTime = Time.time;
            if (Input.anyKey && runonce == true)
            {
                runonce = false;
            }
        } 
        else
        {
            time = Time.time - startTime;

            minutes = ((int)time / 60).ToString();

            if ((time % 60) < 10)
            {
                seconds = "0" + (time % 60).ToString("f1");
            } 
            else
            {
                seconds = (time % 60).ToString("f1");
            }

            text.text = "Time: " + minutes + ":" + seconds;
        }
    }
}
