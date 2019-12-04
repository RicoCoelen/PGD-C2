using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{
    public GameObject hook;

    public GameObject curHook;

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
        if (Input.GetMouseButtonDown(0) && active)
        {

            if(curHook.GetComponent<HookScript>().child != null)
                curHook.GetComponent<HookScript>().child.parent = null;

            Destroy(curHook);

            active = false;
        }else if (Input.GetMouseButtonDown(0) && !active)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            curHook = Instantiate(hook, transform.position, Quaternion.identity);
            GetComponent<newChainScript>().chainAnchor = curHook;

            curHook.GetComponent<ChainScript>().direction = direction;

            active = true;
        }else if (Input.GetButtonDown("Jump") && active && curHook.GetComponent<HookScript>().child.tag == "Anchored Grabable")
        {
            playerScript.MovementJump();


            if (curHook.GetComponent<HookScript>().child != null)
                curHook.GetComponent<HookScript>().child.parent = null;



            Destroy(curHook);

            active = false;
        }
    }
}
