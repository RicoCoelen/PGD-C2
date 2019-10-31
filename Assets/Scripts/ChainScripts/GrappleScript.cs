using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    public GameObject grappleHead;
    private float timeToShoot;
    public float cooldownShoot;
    public GameObject lastHit;
    public Transform lastHitPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateAndPosition();
        checkForShoot();
    }

    void RotateAndPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(
            mousePosition.x - transform.position.x,
            mousePosition.y - transform.position.y
            );

        transform.GetChild(0).transform.position = mousePosition;

        transform.up = direction;
    }

    void checkForShoot()
    {
        // shooting
        if (timeToShoot <= 0)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                GameObject temp = Instantiate(grappleHead, transform.GetChild(1).position, transform.GetChild(1).transform.rotation);
                temp.transform.Rotate(0, 90, 0);
                temp.transform.parent = transform.parent;
            }
            timeToShoot = cooldownShoot;
        }
        else
        {
            timeToShoot -= Time.deltaTime;
        }

    }
}
