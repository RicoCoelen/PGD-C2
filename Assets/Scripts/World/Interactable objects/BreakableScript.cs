using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableScript : MonoBehaviour
{

    public GameObject[] brokenPrefabs;

    public void Break()
    {
        // instantiate random broken prefab
        GameObject BrokenPrefab = Instantiate(brokenPrefabs[Random.Range(0, brokenPrefabs.Length)], transform.position, Quaternion.identity);

        // destroy current object
        Destroy(gameObject);
    }
}
