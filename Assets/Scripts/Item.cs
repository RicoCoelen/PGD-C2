using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private GameObject player;
    
    public Texture icon;

    public string type;

    public float decreaseRate;

    public bool pickedUp;
    public bool equiped;

    public Vector3 itemPosition;
    public Vector3 itemRotation;
    public Vector3 itemScale;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        if (equiped)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                UnEquip();
        }
    }

    public void UnEquip()
    {
        player.GetComponent<PlayerStats>().weaponEquipped = false;
        equiped = false;
        this.gameObject.SetActive(false);
    }
}
