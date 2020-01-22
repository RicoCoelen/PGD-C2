using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ButtonSwitches : MonoBehaviour
{
    [SerializeField]
    private GameObject switchOn, switchOff;
    public GameObject player;
    [SerializeField]
    public GameObject door;
    Light2D light2d;


    public bool isOn = false;

    private void Start()
    {
        light2d = GetComponentInChildren<Light2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(isOn == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;
            light2d.color = Color.red;
        }

        if(isOn == true)
        {
            light2d.color = Color.green;
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void switchState() //Wordt aangeroepen wanneer de chain collision heeft met de switch
    {
        AudioManager.PlaySound("OpenDoor");

        if(isOn == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<SpriteRenderer>().sprite = door.GetComponent<DoorScript>().doorOpen.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<BoxCollider2D>().enabled = false;
            isOn = !isOn;
            light2d.color = Color.green;
        }
        else if(isOn == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<SpriteRenderer>().sprite = door.GetComponent<DoorScript>().doorClosed.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<BoxCollider2D>().enabled = true;
            isOn = !isOn;
            light2d.color = Color.red;
        }
    }
    public void Update()
    {
        
    }
}
