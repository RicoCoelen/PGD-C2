using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool hovered;
    public bool empty;

    public GameObject item;
    public Texture itemIcon;

    private GameObject player;

    private void Start()
    {
        hovered = false;
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (item)
        {
            empty = false;

            itemIcon = item.GetComponent<Item>().icon;
            this.GetComponent<RawImage>().texture = itemIcon;
        }
        else
        {
            empty = true;
            itemIcon = null;
            this.GetComponent<RawImage>().texture = null;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item)
        {
            Item thisItem = item.GetComponent<Item>();

            // checking item type
            if (thisItem.type == "Drinkable")
            {
                Debug.Log("Thirst " + thisItem.decreaseRate);
                player.GetComponent<PlayerStats>().Drink(thisItem.decreaseRate);
                Destroy(item);

            }
            if (thisItem.type == "Eatable")
            {
                Debug.Log("Food " + thisItem.decreaseRate);
                player.GetComponent<PlayerStats>().Eat(thisItem.decreaseRate);
                Destroy(item);
            }
            if (thisItem.type == "Med")
            {
                Debug.Log("Med " + thisItem.decreaseRate);
                player.GetComponent<PlayerStats>().Heal(thisItem.decreaseRate);
                Destroy(item);
            }
            if (thisItem.type == "Weapon" && !player.GetComponent<PlayerStats>().weaponEquipped)
            {
                Debug.Log("Weapon" + thisItem.name);
                thisItem.equiped = true;
                item.SetActive(true);
                player.GetComponent<PlayerStats>().weaponEquipped = true;
            }
        }
    }
}
