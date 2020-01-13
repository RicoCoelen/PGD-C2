using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitches : MonoBehaviour
{
    [SerializeField]
    private GameObject switchOn, switchOff;
    public GameObject player;
    [SerializeField]
    public GameObject door;
    
    public bool isOn = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(isOn == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;
        }

        if(isOn == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void switchState() //Wordt aangeroepen wanneer de chain collision heeft met de switch
    {
        if(isOn == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<SpriteRenderer>().sprite = door.GetComponent<Door>().doorOpen.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<BoxCollider2D>().enabled = false;
            isOn = !isOn;
        }
        else if(isOn == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<SpriteRenderer>().sprite = door.GetComponent<Door>().doorClosed.GetComponent<SpriteRenderer>().sprite;
            door.GetComponent<BoxCollider2D>().enabled = true;
            isOn = !isOn;
        }
        Debug.Log(isOn + "ison");
    }
    public void Update()
    {
        
    }
}
