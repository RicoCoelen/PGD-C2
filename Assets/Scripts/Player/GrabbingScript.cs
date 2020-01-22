using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingScript : MonoBehaviour
{
    // current item
    public GameObject currentItem = null;
    public Rigidbody2D currentItemRigidBody = null;
    // item place
    public Transform ItemHolder;
    // throw force
    public float forceMultiplier = 1f;
    public float torque = 1f;
    // coroutine
    private IEnumerator coroutine;

    // Update is called once per frame
    void Update()
    {
        if (currentItem != null)
        {
            if (GetComponent<ThrowHook>().firstHook != null && GetComponent<ThrowHook>().firstHook.TryGetComponent<HookScript>(out HookScript hs))
            {
                if (hs != null && hs.child != null)
                {
                    // get current item connected to chain
                    GameObject connectedObject = hs.child.gameObject;

                    // if grabable
                    if (connectedObject != null)
                    {
                        connectedObject.transform.parent = null;
                        Destroy(GetComponent<ThrowHook>().firstHook);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ThrowItem();
            }
            else
            {
                // make player parent of item
                currentItem.transform.parent = transform.parent;

                // change position / rotation of item to holder
                currentItem.transform.position = ItemHolder.transform.position;
                currentItem.transform.rotation = ItemHolder.transform.rotation;
            }
        }
        else
        {
            if (GetComponent<ThrowHook>().firstHook != null && GetComponent<ThrowHook>().firstHook.TryGetComponent<HookScript>(out HookScript hs))
            {
                if (hs != null && hs.child != null)
                {
                    // get current item connected to chain
                    GameObject connectedObject = hs.child.gameObject;

                    // if grabable
                    if (connectedObject != null)
                    {
                        switch(connectedObject.tag)
                        {
                            case "Grabable":
                                GrabItem(connectedObject);
                                connectedObject.transform.parent = null;
                                Destroy(GetComponent<ThrowHook>().firstHook);
                                break;
                        }
                    }
                }
            }
        }
    }

    private void ToggleCollision(GameObject go, bool isTrue)
    {
        // check if item has rigidbody
        if (go.TryGetComponent<Collider>(out Collider collider))
        {
            Debug.Log(isTrue);
            if (collider != null)
            {
                // find a way to temporarily disable collision with player
                //Physics.IgnoreCollision(collider, GetComponent<Collider>(), isTrue);
                //Debug.Log(isTrue);
            }
        }
    }

    private void ToggleRigidBody(GameObject go)
    {
        // check if item has rigidbody
        if (go.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            if (rb2d != null)
            {
                rb2d.simulated = !rb2d.simulated;
            }
        }
    }

    public void GrabItem(GameObject item)
    {
        // toggle simulation, toggle collison object
        ToggleRigidBody(item);
        ToggleCollision(item, true);

        currentItem = item;
        // check if item has rigidbody
        if (currentItem.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            if (rb2d != null)
            {
                currentItemRigidBody = rb2d;
            }
        }
    }

    public void ThrowItem()
    {
        // toggle physics simulation
        ToggleRigidBody(currentItem);
        // add collision back
        //coroutine = TurnCollisionOn(currentItem);
        //StartCoroutine(coroutine);

        // check if current item has rigidbody
        if (currentItemRigidBody != null)
        {
            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (Vector2) (mousepos - currentItem.transform.position).normalized;
            currentItemRigidBody.AddTorque(torque, 0);
            currentItemRigidBody.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
        }

        // clean current item
        currentItem = null;
        // clean current rigidbody after thrown
        currentItemRigidBody = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // grab item if collides with player collier
        //// check if holding item
        //if (currentItem == null)
        //{
        //    // do according to gameobject tag
        //    switch (collision.gameObject.tag)
        //    {
        //        case "Grabable":
        //            GrabItem(collision.gameObject);
        //            break;

        //        case "Enemy":
        //            GrabItem(collision.gameObject);
        //            break;
        //    }
        //}
    }

    IEnumerator TurnCollisionOn(GameObject go)
    {
        // YieldInstruction waits for 1 seconds.
        yield return new WaitForSeconds(1);

        // toggle collision back on
        ToggleCollision(go, false);
    }
}