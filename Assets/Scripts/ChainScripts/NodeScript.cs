using UnityEngine;

public class NodeScript : MonoBehaviour
{
    ChainScript hook;
    DistanceJoint2D hookJoint;
    Rigidbody2D player;

    public bool hinge = false;
    public bool skipped = false;

    /// <summary>
    /// Getting and finding components so we don't have to do it every frame.
    /// </summary>
    void Start()
    {
        // Getting the chainscript of the hook so we dont have to find it every time a node collides.
        hook = GameObject.FindGameObjectWithTag("Hook").GetComponent<ChainScript>();
    }

    /// <summary>
    /// Checks if the node collides with the gameobject layer of the level and if the hook isFlexible.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If collides is true, set hinge to true. Meaning the chainscript is going to set the distance joint there.
        if (collision.gameObject.layer == 9 && hook.isFlexible)
        {
            hinge = true;
        }
    }
}
