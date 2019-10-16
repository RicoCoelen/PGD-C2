using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public float health;

    public GameObject[] item;

    public bool dead;

    public void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void DropItems()
    {
        for (int i = 0; i < item.Length; i++)
        {
            GameObject droppedItem = Instantiate(item[i], transform.position, Quaternion.identity);
        }
    }

    public void Die()
    {
        DropItems();
        Destroy(this.gameObject);
    }
}
