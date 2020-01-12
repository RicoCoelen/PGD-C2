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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        //gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
        //isOn = true;

    }

    public void switchState()
    {
        if(isOn == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
            isOn = true;
        }

        if(isOn == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;
            isOn = false;
        }
    }
    public void Update()
    {
        
    }
}
