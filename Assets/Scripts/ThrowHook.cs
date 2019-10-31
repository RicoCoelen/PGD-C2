using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{
    public GameObject hook;

    public GameObject curHook;

    public bool active = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



        if (Input.GetMouseButtonDown(0))
        {
            if (!active) {
                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                curHook = Instantiate(hook, transform.position, Quaternion.identity);

                curHook.GetComponent<ChainScript>().direction = direction;

                active = true;
            }

        }
        if (Input.GetKeyDown("space") && active)
        {
            Debug.Log(curHook);
            if(curHook.GetComponent<HookScript>().child != null)
                curHook.GetComponent<HookScript>().child.parent = null;

            Destroy(curHook);

            active = false;
        }

    }
}
