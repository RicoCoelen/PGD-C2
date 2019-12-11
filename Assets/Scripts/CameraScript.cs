using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    public Transform[] points;
    private int destPoint = 0;
    Vector3 currentdestination;
    public float maxDistance;
    public float speed;
    public CinemachineVirtualCamera vcam;
    public GameObject Player;
    public GameObject Canvas;
    private bool runonce = true;

    // Start is called before the first frame update
    void Start()
    {
        GotoNextPoint();
        Player = GameObject.FindWithTag("Player");
        vcam.Follow = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runonce == true)
        {
            // Choose the next destination point when the agent gets
            transform.position = Vector3.MoveTowards(transform.position, currentdestination, speed);

            // close to the current one.
            if (Vector3.Distance(transform.position, currentdestination) < maxDistance)
                GotoNextPoint();

            if (runonce == true && Input.anyKey)
            {
                vcam.Follow = Player.transform;
                runonce = false;
                Canvas.SetActive(false);
            }
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        currentdestination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }
}
