using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    float startTime;
    float time;
    public string minutes;
    public string seconds;
    private bool runonce = true;

    [SerializeField] Text parTime;
    public string currentParTime = "";

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        // Only show the Par time UI if there is a par time set
        if (!string.IsNullOrWhiteSpace(currentParTime))
        parTime.GetComponent<Text>().text = " Dev Time: " + currentParTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runonce == true)
        {
            if (Input.anyKey && runonce == true)
            {
                runonce = false;
            }
        } 
        else
        {
            time = Time.time - startTime;

            minutes = ((int)time / 60).ToString();
            seconds = (time % 60).ToString("f1");

            gameObject.GetComponent<Text>().text = "Time: " + minutes + ":" + seconds;
        }
    }
}
