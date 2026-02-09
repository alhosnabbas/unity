using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthAllObjects : MonoBehaviour
{

    private float health; // Initial health value

    private float maxHealth = 500f; // Maximum health value

    // Method to apply damage to the object

    void Start()
    {

        health = maxHealth;
    }

    void Update()
    {

    }
    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        //Debug.Log("Health: " + health);
        if (health <= 0)
        {
            DestroyObject();
        }


    }

    // Method to destroy the object when health reaches zero
    private void DestroyObject()
    {
        // Logic to destroy the object (e.g., play animation, remove from scene, etc.)
        //Debug.Log("Object destroyed!");
        Destroy(gameObject); // Uncomment this line if this script is attached to a GameObject
    }

}
