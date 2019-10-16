using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    // Variables
    public float maxHealth, maxThirst, maxHunger;
    public float thirstIncreaseRate, hungerIncreaseRate;
    [HideInInspector] public float health, thirst, hunger;

    public Image fillerHealth, fillerHunger, fillerThirst;

    public bool playerDead;
    public bool weaponEquipped;
    public float damage;
    public static bool triggeringWithAI;
    public static GameObject aiObject;

    public static bool triggeringWithTree;
    public static GameObject treeObject;


    // Functions
    public void Start()
    {
        health = maxHealth;
        thirst = maxThirst;
        hunger = maxHunger;

        weaponEquipped = false;
    }

    public void FixedUpdate()
    {
        // increase thirst and hunger
        if (!playerDead)
        {
            if(thirst >= 0)
                thirst -= thirstIncreaseRate * Time.deltaTime;
            if(hunger >= 0)
                hunger -= hungerIncreaseRate * Time.deltaTime;
        }

        // check if thirst or hunger are passed max
        // if so, decreases health
        if (thirst <= 0 && !playerDead)
        {
            health -= 1 * Time.deltaTime;

            //Debug.Log("You are dieing of thirst");
        }
        if (hunger <= 0 && !playerDead)
        {
            health -= 1 * Time.deltaTime;
            //Debug.Log("You are dieing of hunger");
        }

        // Check if health reached 0
        if (health <= 0)
        {
            Die();
            Time.timeScale = 0;
        }

        // Tree Chopping
        if (triggeringWithTree && treeObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack(treeObject);
            }
        }

        // Collision AI
        if (triggeringWithAI && aiObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack(aiObject);
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        fillerHealth.fillAmount = (health / maxHealth);
        fillerThirst.fillAmount = (thirst / maxThirst);
        fillerHunger.fillAmount = (hunger / maxHunger);
    }
    public void Die()
    {
        playerDead = true;
        Debug.Log("You have died because of thirst or hunger");
        //Time.timeScale = 0f;
    }

    public void Drink(float decreaseRate)
    {
        thirst += decreaseRate;
    }
    public void Eat(float decreaseRate)
    {
        hunger += decreaseRate;
    }

    public void Heal(float decreaseRate)
    {
        health += decreaseRate;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Animal")
        {
            aiObject = other.gameObject;
            triggeringWithAI = true;
        }
        if (other.tag == "Tree")
        {
            triggeringWithTree = true;
            treeObject = other.gameObject;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Animal")
        {
            aiObject = null;
            triggeringWithAI = false;
        }
        if (other.tag == "Tree")
        {
            triggeringWithTree = false;
            treeObject = null;
        }
    }

    public void Attack(GameObject target)
    {
        if (target.tag == "Animal" && weaponEquipped)
        {
            Animal animal = target.GetComponent<Animal>();
            animal.health -= damage;
        }
        if (target.tag == "Tree" && weaponEquipped)
        {
            Tree tree = target.GetComponent<Tree>();
            tree.health -= damage;
        }
    }
}
