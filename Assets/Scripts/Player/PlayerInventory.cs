using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject slotHolder;
    public GameObject itemManager;
    private bool inventoryEnabled;

    private int slots;
    private Transform[] slot;

    private GameObject itemPickedUp;
    private bool itemAdded;

    public void Start()
    {
        // slots being detected
        slots = slotHolder.transform.childCount;
        slot = new Transform[slots];
        detectInventorySlots();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryEnabled = !inventoryEnabled;
        }

        if (inventoryEnabled)
        {
            inventory.GetComponent<Canvas>().enabled = true;
            Time.timeScale = 0;
        }
        else
        {
            inventory.GetComponent<Canvas>().enabled = false;
            Time.timeScale = 1;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            itemPickedUp = other.gameObject;
            AddItem(itemPickedUp);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Item")
        {
            itemAdded = false;
        }
    }

    public void AddItem(GameObject item)
    {
        for (int i = 0; i < slots; i++)
        {
            if (slot[i].GetComponent<Slot>().empty && !itemAdded)
            {
                Debug.Log("Saved " + slot[i]);
                slot[i].GetComponent<Slot>().item = itemPickedUp;
                slot[i].GetComponent<Slot>().itemIcon = itemPickedUp.GetComponent<Item>().icon;

                item.transform.parent = itemManager.transform;
                item.transform.position = itemManager.transform.position;

                item.transform.localPosition = item.GetComponent<Item>().itemPosition;
                item.transform.localEulerAngles = item.GetComponent<Item>().itemRotation;
                item.transform.localScale = item.GetComponent<Item>().itemScale;

                item.GetComponent<Item>().pickedUp = true;
                Destroy(item.GetComponent<Rigidbody>());

                itemAdded = true;

                item.SetActive(false);
            }
        }
    } 

    public void detectInventorySlots()
    {

        for(int i = 0; i < slots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i);
        }

    }
}
