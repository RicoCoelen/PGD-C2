using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ChainScript : MonoBehaviour
{
    [Header("Length")]
    public float maxDistance = 5;
    public float minDistance = 5;

    [Header("Travel")]
    public float speed = 1;
    public float interfal = 2;

    [Header("Climbing")]
    public float climbSpeed = 20f;

    [Header("Game Objects")]
    public GameObject node;

    [Header("Layers")]
    public LayerMask noHookLayer;
    
    [System.NonSerialized] public bool isFlexible;
    [System.NonSerialized] public Vector2 direction;
    [System.NonSerialized] public DistanceJoint2D chainJoint;
    [System.NonSerialized] public GameObject lastNode;

    private PlayerScript playerScript;
    private Vector2 startingPoint;
    private int lastNodesCount;
    private bool done = false;
    private bool stop = false;
    private bool soundPlayed = false;
    private GameObject player;
    
    private List<GameObject> nodes;


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
        if (isFlexible)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
            chainJoint.distance = Vector2.Distance(player.transform.position, GetComponent<Rigidbody2D>().position);
            chainJoint.connectedBody = player.GetComponent<Rigidbody2D>();
            chainJoint.enabled = true;

            Debug.DrawLine(player.transform.position, GetComponent<Rigidbody2D>().position, Color.green);
        }
        else
        {
            soundPlayed = false;
        }

        // Old hook script
        {
            // Makes the hook move in the clicked direction and creating nodes for in the chains
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
                }
            }

            // Handles chain length
            {
                // Changes distance joint length
                if (Input.GetButton("down") && chainJoint.distance != minDistance)
                {
                    chainJoint.distance += climbSpeed * Time.deltaTime;
                }
                else if (chainJoint.distance < minDistance)
                    chainJoint.distance = minDistance;

                if (Input.GetButton("up") && chainJoint.distance != maxDistance)
                {
                    chainJoint.distance -= climbSpeed * Time.deltaTime;
                }
                else if (chainJoint.distance >= maxDistance)
                    chainJoint.distance = maxDistance;

                // Changes chain length if necessary
                if (isFlexible && done && NodeCheck())
                {
                    ChangeLength();
                }

                lastNodesCount = nodes.Count;

            }

            HitWall();
        }
    }

    /// <summary>
    /// Checks if the hook hit a wall and stops it from traveling
    /// </summary>
    void HitWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
        // Maybe make a layer for everything you can actually hook
        if (hit.collider != player.GetComponent<Collider2D>() && hit.collider != null && hit.collider.gameObject.tag != "Breakable Wall")
        {
            RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + (Vector2.up * 0.2f), Vector2.up, 0.1f);
            if (hit2.collider != player.GetComponent<Collider2D>() && hit2.collider != null && hit2.collider.gameObject.layer != 15 && hit2.collider.gameObject.tag != "Switch" && hit2.collider.gameObject.tag != "Breakable Wall")
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