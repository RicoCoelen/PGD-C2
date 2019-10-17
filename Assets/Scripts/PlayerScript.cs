using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    public void PlayerMovement()
    {
        Vector3 playerMovement = new Vector3(Input.GetAxis("Horizontal"), 0f);
        transform.position += playerMovement * Time.deltaTime * moveSpeed;
        

        
    }
}
