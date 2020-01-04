using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingScript : MonoBehaviour
{
    public GameObject currentItem = null;
    public Rigidbody2D currentItemRigidBody = null;
    public Transform ItemHolder;
    public float forceMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItem != null)
        { 
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
    }

    private void ToggleCollision(GameObject go)
    {
        // check if item has rigidbody
        if (go.TryGetComponent<Collider2D>(out Collider2D collider))
        {
            if (collider != null)
            {
                collider.enabled = !collider.enabled;
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
        ToggleRigidBody(item);
        ToggleCollision(item);

        currentItem = item;
    }

    public void ThrowItem()
    {
        ToggleRigidBody(currentItem);

        GameObject tempitem = currentItem;
        currentItem = null;

        if (tempitem.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            if (rb != null)
            {
                Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (Vector2) (mousepos - tempitem.transform.position).normalized;
                rb.AddForce(direction * forceMultiplier);
               
            }
        }

        // add collision back
        ToggleCollision(tempitem);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentItem == null)
        {
            switch (collision.gameObject.tag)
            {
                case "Grabable":
                    GrabItem(collision.gameObject);
                    break;

                case "Enemy":
                    GrabItem(collision.gameObject);
                    break;
            }
        }
    }
}
