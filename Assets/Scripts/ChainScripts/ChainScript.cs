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
        WrappingChain();
    }

    /// <summary>
    /// This part of the code is handeling the wrapping and unwrapping of the chain, so it can swing around corners aswell.
    /// This is done by looping through all of the list with notes and then activating or deactivating a DistanceJoint2D
    /// </summary>
    void WrappingChain()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            // Checks if the node is the anchor, then continue
            if (nodes[i].GetComponent<NodeScript>() == null)
            {
                continue;
            }
            // Checks if the bool is true, which is set true or false in the NodeScript.
            else if (nodes[i].GetComponent<NodeScript>().hinge)
            {
                // If the next note is not empty it will run this because otherwise it will have missing variables.
                if (nodes[i + 1] != null)
                {
                    // This if else script is there to determine if anything should happen. Since we only want to have a distance joint between the player and the last node that is touching part of the level.
                    if (nodes[i + 1].GetComponent<NodeScript>().hinge)
                    {
                        nodes[i].GetComponent<NodeScript>().skipped = true;
                    }
                    else
                    {
                        nodes[i].GetComponent<DistanceJoint2D>().enabled = true;
                        nodes[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

                        chainJoint.enabled = false;

                        // Because we want to check from the player to the hook, we flip the for loop, so we start on the last node. 
                        for (int j = i; j > 0; j--)
                        {
                            if (nodes[j].GetComponent<NodeScript>() == null)
                            {
                                break;
                            }
                            // In this else if we set all the nodes to disable their DistanceJoint2D. So that we can enable the right one.
                            // After doing that, we set the connectedBody to the player and set the distance to the distance in between.
                            // We only enable the DistaneJoint2D for that node.
                            else if (nodes[j].GetComponent<NodeScript>().skipped == false)
                            {
                                foreach (GameObject node in nodes)
                                {
                                    node.GetComponent<DistanceJoint2D>().enabled = false;
                                }

                                Debug.Log("Node " + i + " should wrap");

                                nodes[i].GetComponent<DistanceJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
                                nodes[i].GetComponent<DistanceJoint2D>().distance = Vector3.Distance(nodes[i].GetComponent<Rigidbody2D>().position, player.GetComponent<Rigidbody2D>().position);
                                nodes[i].GetComponent<DistanceJoint2D>().enabled = true;
                            }
                        }
                        
                        // For the unwrapping the code compares the node position and player position and if the player y is smaller then the node y
                        // It disables the DistanceJoint2D
                        if (nodes[i].GetComponent<Rigidbody2D>().position.y > player.GetComponent<Rigidbody2D>().position.y)
                        {
                            Debug.Log("Node " + i + " should unwrap");

                            // For all the nodes we reset the nodes back to a beginning state.
                            foreach (GameObject node in nodes)
                            {
                                nodes[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                                nodes[i].GetComponent<DistanceJoint2D>().enabled = false;
                                nodes[i].GetComponent<NodeScript>().hinge = false;
                            }

                            // And then set the hook to the player again.
                            chainJoint.connectedBody = player.GetComponent<Rigidbody2D>();
                            chainJoint.distance = Vector3.Distance(chainJoint.GetComponent<Rigidbody2D>().position, player.GetComponent<Rigidbody2D>().position);
                            if (chainJoint.distance > maxDistance)
                            {
                                chainJoint.distance = maxDistance;
                            }
                            chainJoint.enabled = true;
                        }
                    }
                }
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
            if (hit2.collider != player.GetComponent<Collider2D>() && hit2.collider != null && hit2.collider.gameObject.layer != 15 && hit2.collider.gameObject.tag != "Switch" && hit2.collider.tag != "Grabable")
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