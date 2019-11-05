using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    float startTime;
    float time;
    string minutes;
    string seconds;

    [SerializeField] Text parTime;
    public string currentParTime = "";

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        parTime.GetComponent<Text>().text = " Par: " + currentParTime;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.time - startTime;

        minutes = ((int) time / 60).ToString();
        seconds = (time % 60).ToString("f1");

        gameObject.GetComponent<Text>().text = "Time: " + minutes + ":" + seconds;
    }
}
