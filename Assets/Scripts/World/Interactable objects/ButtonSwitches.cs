using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitches : MonoBehaviour
{
    [SerializeField]
    private GameObject switchOn, switchOff;
    public GameObject player;
    
    public bool isOn = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.GetComponent<SpriteRenderer>().sprite = switchOff.GetComponent<SpriteRenderer>().sprite;

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("collision button");
        }

        gameObject.GetComponent<SpriteRenderer>().sprite = switchOn.GetComponent<SpriteRenderer>().sprite;
        isOn = true;

    }
}
