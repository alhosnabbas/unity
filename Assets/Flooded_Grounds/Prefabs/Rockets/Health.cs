using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Make sure to include this for UI components
public class Health : MonoBehaviour
{
    private float health; // Initial health value
    public Slider healthSlider;
    public float maxHealth = 500f; // Maximum health value
    private LauncherManager launcherManager;
    // Method to apply damage to the object

    void Start()
    {

        health = maxHealth;
        launcherManager = FindFirstObjectByType<LauncherManager>();
    }

    void Update()
    {
        if (healthSlider != null && healthSlider.value != health)
        {
            healthSlider.value = health;
        }
    }
    public void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        //Debug.Log("Health: " + health);
        if (health <= 0)
        {
            DestroyObject();
        }

        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
        else
        {
            // Debug.LogWarning("Health slider is not assigned!");
        }
    }

    // Method to destroy the object when health reaches zero
    private void DestroyObject()
    {
        Launcher launcher = GetComponent<Launcher>();
        if (launcherManager != null && launcher != null)
        {
            launcherManager.RemoveLauncher(launcher);
            Destroy(gameObject);
        }

    }

}
