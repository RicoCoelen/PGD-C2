using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject thisItem;
    public GameObject[] requiredItem;
    
     
    private bool hovered;
    private GameObject player;
    private GameObject itemManager;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        itemManager = GameObject.FindGameObjectWithTag("ItemManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (hovered)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //perform things, check for required items
                CheckForItems();
            }
        }
    }

    public void CheckForItems()
    {
        int itemsInManager = itemManager.transform.childCount;

        if(itemsInManager > 0)
        {
            Debug.Log("Start Check");
            int itemsFound = 0;

            for (int i = 0; i < itemsInManager; i++)
            {
                for (int j = 0; j < requiredItem.Length; j++)
                {
                    Debug.Log(itemManager.transform.GetChild(i).name);
                    if (itemManager.transform.GetChild(i).name == requiredItem[j].name)
                    {
                        itemsFound++;
                        break;
                    }
                }
            }
            Debug.Log("Eind Check");
            if (itemsFound >= requiredItem.Length)
            {
                

                Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);
                GameObject spawnedItem = Instantiate(thisItem, playerPos, Quaternion.identity);
                player.GetComponent<PlayerInventory>().AddItem(spawnedItem);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
