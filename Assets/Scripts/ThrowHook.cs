using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{
    public GameObject hook;

    public GameObject curHook;

    public bool active = false;

    [SerializeField] GameObject player;
    PlayerScript playerScript;

    // Use this for initialization
    void Start()
    {
        playerScript = player.GetComponent<PlayerScript>();
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
        if (Input.GetButtonDown("Jump") && active)
        {
            Debug.Log(curHook);

            //Vector2 momentum = playerScript.rb.velocity;

            if(curHook.GetComponent<HookScript>().child != null)
                curHook.GetComponent<HookScript>().child.parent = null;

            GetComponent<PlayerScript>().MovementJump();

            Destroy(curHook);

            //playerScript.rb.velocity = momentum;

            active = false;
        }

    }
}
