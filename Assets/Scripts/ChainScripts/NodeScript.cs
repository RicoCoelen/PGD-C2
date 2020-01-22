using UnityEngine;

public class NodeScript : MonoBehaviour
{
    GameObject hook;
    DistanceJoint2D hookJoint;
    Rigidbody2D player;

    public bool hinge = false;
    public bool skipped = false;

    // Start is called before the first frame update
    void Start()
    {
        hook = GameObject.FindGameObjectWithTag("Hook");
        hookJoint = hook.GetComponent<DistanceJoint2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check of de node collide,
        // Check of de node de laatste node is die collide,
        // Als het de laatste is, enable de distancejoint,
        // Zet de connectedbody van de node aan de speler,
        // Zet de connectedbody van de hook aan de speler,
        // 
        // List is ordered



        //als ie met tile colide

        if (collision.gameObject.layer == 9 && hook.GetComponent<ChainScript>().isFlexible)
        {
            hinge = true;
        }



        //connecten we die node naar de player
        //en de hook naar die node.
    }
}
