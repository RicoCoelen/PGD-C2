using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{

    public GameObject brokenBits;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Node":
            case "Hook":
                Debug.Log("Hook + Break");
                BreakWall();
                Destroy(GameObject.FindGameObjectWithTag("Hook"));
                break;
            case "Grabable":
                BreakWall();
                collision.gameObject.GetComponent<BreakableScript>().Break();
                break;
        }
    }

    public void BreakWall()
    {
        GameObject brokenBox = Instantiate(brokenBits, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
