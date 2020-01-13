using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject doorClosed;
    public GameObject doorOpen;
    public bool isopen;
    // Start is called before the first frame update
    void Start()
    {
        if(isopen == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = doorOpen.GetComponent<SpriteRenderer>().sprite;
        }

        if(isopen == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = doorClosed.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void openDoor()
    {
        
        if (isopen == false)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = doorOpen.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            isopen = true;
        }

        if (isopen == true)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = doorClosed.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            isopen = false;
        }
        Debug.Log(isopen + "isopen");
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
