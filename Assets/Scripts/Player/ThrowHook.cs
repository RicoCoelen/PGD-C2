using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{
    public GameObject hook;

    public GameObject firstHook;
    public GameObject secondHook;

    public bool active = false;

    GameObject player;
    PlayerScript playerScript;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (Time.timeScale == 1)
        {
            if (Input.GetMouseButtonUp(0) && firstHook != null)
            {

                if (firstHook.GetComponent<HookScript>().child != null)
                    firstHook.GetComponent<HookScript>().child.parent = null;

                player.GetComponent<PlayerScript>().ChainJump();

                Destroy(firstHook);

                active = false;
            }
            else if (Input.GetMouseButtonDown(0) && firstHook == null)
            {
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                firstHook = Instantiate(hook, transform.position, Quaternion.identity);

                firstHook.GetComponent<ChainScript>().direction = direction;

                firstHook.GetComponent<HookScript>().inputButton = 0;

                active = true;
            }
        }
    }
}
