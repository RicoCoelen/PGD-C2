using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class newChainScript : MonoBehaviour
{

    // Instance the properties of ChainAnchor
    public GameObject chainAnchor;
    public DistanceJoint2D chainJoint;
    public PlayerScript playerScript;
    public LineRenderer chainRenderer;
    public LayerMask chainLayerMask;
    public float chainMaxCastDistance = 20f;
    public float climbSpeed = 20f;

    public bool chainAttached;
    Vector2 playerPosition;
    Rigidbody2D chainAnchorRb;
    SpriteRenderer chainAnchorSprite;
    List<Vector2> chainPositions = new List<Vector2>();
    bool distanceSet;
    bool isColliding;

    private void Awake()
    {
        // Disable the chainJoint and get the player position and components of the chainAnchor at the start of the game.
        chainJoint.enabled = false;
        playerPosition = transform.position;
        chainAnchorRb = chainAnchor.GetComponent<Rigidbody2D>();
        chainAnchorSprite = chainAnchor.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the mouse position and use it to determine the angle of the chain throw
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 facingDirection = mousePosition - new Vector2(transform.position.x, transform.position.y);
        float aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);

        // keep the aimAngle positive by using Pi * 2
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        Vector2 aimDirection = Quaternion.Euler(0,0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        playerPosition = transform.position;

        HandleInput(aimDirection);

        if (chainAttached)
        {
            UpdateRopePositions();
            HandleChainLength();
        }
    }

    void HandleInput(Vector2 aimDirection)
    {
        // Throw the chain
        if (Input.GetButtonDown("chainThrow") && !chainAttached)
        {
            RaycastHit2D hit = Physics2D.Raycast(playerPosition, aimDirection, chainMaxCastDistance, chainLayerMask);

            if (hit.collider != null)
            {
                chainAttached = true;
                if (!chainPositions.Contains(hit.point))
                {
                    // Jump slightly to distance the player a little from the ground after grappling to something.
                    transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 2f), ForceMode2D.Impulse);
                    chainPositions.Add(hit.point);
                    chainJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    chainJoint.enabled = true;
                    chainAnchorSprite.enabled = true;
                    chainRenderer.enabled = true;
                }
            }
            else
            {
                chainRenderer.enabled = false;
                chainAttached = false;
                chainJoint.enabled = false;
            }

        }

        // retract the chain
        if (chainAttached && Input.GetButtonUp("chainThrow"))
        {
            playerScript.chainJump();
            ResetChain();
        }

    }

    public void ResetChain()
    {
        chainJoint.enabled = false;
        chainAttached = false;
        chainRenderer.enabled = false;

        chainRenderer.positionCount = 2;
        chainRenderer.SetPosition(0, transform.position);
        chainRenderer.SetPosition(1, transform.position);

        chainPositions.Clear();
        chainAnchorSprite.enabled = false;
    }

    void UpdateRopePositions()
    {
        //I don't fucking know anymore just fill this in later
        // 2
        chainRenderer.positionCount = chainPositions.Count + 1;

        // 3
        for (int i = chainRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != chainRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                chainRenderer.SetPosition(i, chainPositions[i]);

                // 4
                if (i == chainPositions.Count - 1 || chainPositions.Count == 1)
                {
                    Vector2 chainPosition = chainPositions[chainPositions.Count - 1];
                    if (chainPositions.Count == 1)
                    {
                        chainAnchorRb.transform.position = chainPosition;
                        if (!distanceSet)
                        {
                            chainJoint.distance = Vector2.Distance(transform.position, chainPosition);
                            distanceSet = true;
                        }
                    }
                    else
                    {
                        chainAnchorRb.transform.position = chainPosition;
                        if (!distanceSet)
                        {
                            chainJoint.distance = Vector2.Distance(transform.position, chainPosition);
                            distanceSet = true;
                        }
                    }
                }
                // 5
                else if (i - 1 == chainPositions.IndexOf(chainPositions.Last()))
                {
                    Vector2 chainPosition = chainPositions.Last();
                    chainAnchorRb.transform.position = chainPosition;
                    if (!distanceSet)
                    {
                        chainJoint.distance = Vector2.Distance(transform.position, chainPosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                // 6
                chainRenderer.SetPosition(i, transform.position);
            }
        }
    }

    private void HandleChainLength()
    {
        if (Input.GetButton("up"))
        {
            chainJoint.distance -= Time.deltaTime * climbSpeed;
        }
        else if (Input.GetButton("down") && chainJoint.distance < chainMaxCastDistance)
        {
            chainJoint.distance += Time.deltaTime * climbSpeed;
        }
    }


}