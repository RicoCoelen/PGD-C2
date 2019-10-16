using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Animal : MonoBehaviour
{
    // About the animal
    public float health;

    public GameObject[] item;

    // Animal Navigation
    public float radius;
    public float timer;
    public float idleTimer;
    private bool dead;

    private Transform target;
    private NavMeshAgent agent;
    private float currentTimer;
    private float currentIdleTimer;
    private Animation anim;
    private bool idle;



    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();

        currentTimer = timer;
        currentIdleTimer = idleTimer;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        currentIdleTimer += Time.deltaTime;

        if(currentIdleTimer >= idleTimer)
        {
            StartCoroutine("switchIdle");
        }

        if(currentTimer >= timer && !idle)
        {
            Vector3 newPosition = randomNavSphere(transform.position, radius, -1);
            agent.SetDestination(newPosition);
            currentTimer = 0;
        }

        if (idle)
            anim.CrossFade("idle");
        else
            anim.CrossFade("walk");

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator switchIdle()
    {
        idle = true;
        yield return new WaitForSeconds(3);
        currentIdleTimer = 0;
        idle = false;
    }

    public static Vector3 randomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layerMask);

        return navHit.position;
    }

    public void DropItems()
    {
        for(int i = 0; i < item.Length; i++)
        {
            GameObject droppedItem = Instantiate(item[i], transform.position, Quaternion.identity);
            break;
        }
    }

    public void Die()
    {
        DropItems();
        Destroy(this.gameObject);
    }

    
}
