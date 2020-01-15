using System.Collections.Generic;
using UnityEngine;


public class ChainScript : MonoBehaviour
{
    public float maxDistance = 5;
    public float minDistance = 5;
    public DistanceJoint2D chainJoint;
    private PlayerScript playerScript;
    public LayerMask noHookLayer;

    Vector2 startingPoint;
    public bool isFlexible;

    public Vector2 direction;

    public float speed = 1;
    public float interfal = 2;
    public float climbSpeed = 20f;

    public GameObject node;
    public GameObject player;
    public GameObject lastNode;

    bool done = false;
    bool stop = false;

    public List<GameObject> nodes;

    bool soundPlayed = false;

    // Use this for initialization
    void Start()
    {
        // Initializes everything for old chain
        {
            // Sets references to other objects or scripts
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerScript>();


            // Calculates the direction from start- to endpoint
            startingPoint = player.transform.position;
            direction -= (Vector2)player.transform.position;
            direction.Normalize();
            direction *= maxDistance;
            direction += (Vector2)player.transform.position;

            // Keeps track of all the nodes in the chain
            nodes = new List<GameObject>();
            lastNode = transform.gameObject;
            nodes.Add(lastNode);
        }

        // Initializes everything for the new chain
        {
            chainJoint = GetComponent<DistanceJoint2D>();

            chainJoint.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        if (isFlexible)
        {
            Debug.DrawLine(player.transform.position, GetComponent<Rigidbody2D>().position, Color.green);
        }
        else
        {
            soundPlayed = false;
        }

        HandleTravel();
        SetDistance();
        HitWall();


        // Handles nodes and joints around corners
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<NodeScript>() == null)
            {
                continue;
            }
            else if (nodes[i].GetComponent<NodeScript>().hinge)
            {

                if (nodes[i + 1] != null)
                {
                    if (nodes[i + 1].GetComponent<NodeScript>().hinge)
                    {
                        nodes[i].GetComponent<NodeScript>().skipped = true;
                        //nodes[i].GetComponent<NodeScript>().hinge = false;
                    }
                    else
                    {
                        nodes[i].GetComponent<DistanceJoint2D>().enabled = true;
                        nodes[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

                        chainJoint.GetComponent<DistanceJoint2D>().enabled = false;

                        for (int j = i; j > 0; j--)
                        {
                            if (nodes[j].GetComponent<NodeScript>() == null)
                            {
                                break;
                            }
                            else if (nodes[j].GetComponent<NodeScript>().skipped == false)
                            {
                                foreach (GameObject node in nodes)
                                {
                                    node.GetComponent<DistanceJoint2D>().enabled = false;
                                }

                                Debug.Log("Node " + i + " should connect");

                                nodes[i].GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                                nodes[i].GetComponent<DistanceJoint2D>().distance = Vector3.Distance(nodes[i].GetComponent<Rigidbody2D>().position, player.GetComponent<Rigidbody2D>().position);
                                nodes[i].GetComponent<DistanceJoint2D>().enabled = true;
                            }
                        }
                        // For loop toward the hook to find the last unskipped hinge
                        // Then set its body to this
                        // Set this distanceJoint2D to the player
                    }
                }




                //nodes[j - 1].GetComponent<DistanceJoint2D>().connectedBody = nodes[i].GetComponent<Rigidbody2D>();
                //nodes[j - 1].GetComponent<DistanceJoint2D>().distance = Vector2.Distance(GetComponent<Rigidbody2D>().position, nodes[i - 1].GetComponent<Rigidbody2D>().position);
                //nodes[j].GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            }
        }
    }

    /// <summary>
    /// Makes the hook travel in the given direction until done
    /// </summary>
    void HandleTravel()
    {
        // Moves the hook to
        if (!stop)
            transform.position = Vector2.MoveTowards(transform.position, direction, speed);

        // Creates nodes while hook is traveling
        if (transform.position != (Vector3)direction && !stop)
        {
            if (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
            {
                CreateNode();
            }
        }

        // Creates remaning nodes and attaches player to chain
        else if (done == false)
        {
            done = true;

            while (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal + interfal / 2)
            {
                CreateNode();
            }

            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();

            if (isFlexible)
            {
                transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                chainJoint.distance = Vector2.Distance(player.transform.position, GetComponent<Rigidbody2D>().position);
                chainJoint.connectedBody = player.GetComponent<Rigidbody2D>();
                chainJoint.enabled = true;
            }
        }
    }


    void SetDistance()
    {
        // Decreases the length of the chain
        if (Input.GetButton("down") && chainJoint.distance != minDistance)
        {
            chainJoint.distance += climbSpeed * Time.deltaTime;
        }
        else if (chainJoint.distance < minDistance)
            chainJoint.distance = minDistance;

        // Increases the length of the chain
        if (Input.GetButton("up") && chainJoint.distance != maxDistance)
        {
            chainJoint.distance -= climbSpeed * Time.deltaTime;
        }
        else if (chainJoint.distance >= maxDistance)
            chainJoint.distance = maxDistance;

        // Changes the amount of nodes if chain is to long or short
        if (isFlexible && done && NodeCheck())
        {
            ChangeLength();
        }
    }

    /// <summary>
    /// Checks if the hook hit a wall and stops it from traveling
    /// </summary>
    void HitWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        // Maybe make a layer for everything you can actually hook
        if (hit.collider != player.GetComponent<Collider2D>() && hit.collider != null)
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + (Vector2.up * 0.2f), Vector2.up, 0.1f);
            if (hit2.collider != player.GetComponent<Collider2D>() && hit2.collider != null && hit2.collider.gameObject.layer != 15)
            {
                if (!soundPlayed)
                {
                    AudioManager.PlaySound("ChainHit");
                    soundPlayed = true;
                }


                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                isFlexible = true;
            }
            else
                isFlexible = false;
            stop = true;
        }
    }

    /// <summary>
    /// Initializes nodes and connects it to the rest of the chain
    /// </summary>
    void CreateNode()
    {

        Vector2 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= interfal;
        pos2Create += (Vector2)lastNode.transform.position;

        var playerToHookDirection = (GetComponent<Rigidbody2D>().transform.position - (Vector3)startingPoint).normalized;
        var angle = Mathf.Atan2(playerToHookDirection.y, playerToHookDirection.x) * Mathf.Rad2Deg;



        GameObject go = Instantiate(node, pos2Create, Quaternion.identity);

        go.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        nodes.Add(go);

        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

        lastNode = go;
    }

    /// <summary>
    /// Checks if there are sufficient nodes for the length of the chain
    /// </summary>
    bool NodeCheck()
    {
        if (chainJoint.distance + interfal > nodes.Count * interfal || chainJoint.distance < nodes.Count * interfal)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Changes the chain length by removing or creating nodes
    /// </summary>
    void ChangeLength()
    {
        // Deletes nodes
        if (chainJoint.distance + (4 * interfal) < nodes.Count * interfal)
        {

            int numRemove = (int)(nodes.Count * interfal - chainJoint.distance + (4 * interfal));
            for (int i = 0; i < numRemove; i++)
            {
                GameObject removeNode = nodes[nodes.Count - 1];
                nodes.Remove(removeNode);
                Destroy(removeNode);
                lastNode = nodes[nodes.Count - 1];
            }

            CreateNode();
            CreateNode();

            nodes[nodes.Count - 1].GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }

        // Creates more nodes in necessary
        if (chainJoint.distance > nodes.Count * interfal)
        {
            while (Vector2.Distance(player.transform.position, lastNode.transform.position) > interfal)
            {
                CreateNode();
            }
        }

        lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
    }
}