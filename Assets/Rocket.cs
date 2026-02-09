using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class RocketController : MonoBehaviour
{
    private Rigidbody rb;
    public ParticleSystem explosionEffect; // Assign in Inspector
    public GameObject explosionPrefab; // Assign in Inspector                                       
    public float explosionRadius = 10f;
    public float maxDamage = 100f;
    private Health healthComponent; // For Player or Enemy
    private HealthAllObjects homeHealthComponent; // For Home

    void Start()
    {
        rb = GetComponent<Rigidbody>();



    }

    void Update()
    {
        if (rb.transform.position.y < 0)
        {
            OutOffBounds(); // Destroy the rocket if it falls below y = 0
        }
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            // Rotate to match velocity
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(0, 90, 0);
        }
    }

    void OutOffBounds()
    {

        GameObject sound = Instantiate(explosionPrefab, transform.position, Quaternion.identity);





        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration); // Destroy the effect after it finishes playing
        }

        int mask = LayerMask.GetMask("Player", "Enemy", "Home");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, mask, QueryTriggerInteraction.Collide);

        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the collider is a player or enemy
            if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Home"))
            {
                if (hitCollider.CompareTag("Home"))
                {
                    homeHealthComponent = hitCollider.GetComponent<HealthAllObjects>();
                    if (homeHealthComponent != null)
                    {
                        // Calculate damage based on distance from explosion center
                        float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                        float damage = Mathf.Max(0, maxDamage * (1 - distance / explosionRadius));
                        // Apply damage to the home
                        homeHealthComponent.ApplyDamage(damage);
                        //Debug.Log("Damage dealt: " + damage + " to " + hitCollider.name);
                    }
                }
                else
                {
                    healthComponent = hitCollider.GetComponent<Health>();
                    if (healthComponent != null)
                    {
                        // Calculate damage based on distance from explosion center
                        float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                        float damage = Mathf.Max(0, maxDamage * (1 - distance / explosionRadius));
                        // Apply damage to the player or enemy
                        healthComponent.ApplyDamage(damage);
                        // Debug.Log("Damage dealt: " + damage + " to " + hitCollider.name);
                    }
                }

            }

        }

        Destroy(gameObject); // Destroy the rocket after explosion
    }

    [System.Obsolete]
    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("💥 Rocket exploded at " + transform.position);

        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration);
        }

        // Check ALL layers
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, ~0, QueryTriggerInteraction.Collide);
        // Debug.Log("OverlapSphere found " + hitColliders.Length + " colliders");

        foreach (Collider hitCollider in hitColliders)
        {
            //Debug.Log("Checking collider: " + hitCollider.name + " (Tag: " + hitCollider.tag + ")");

            if (hitCollider.CompareTag("Home"))
            {
                var homeHealthComponent = hitCollider.GetComponent<HealthAllObjects>();
                if (homeHealthComponent != null)
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    float damage = Mathf.Max(0, maxDamage * (1 - distance / explosionRadius));
                    homeHealthComponent.ApplyDamage(damage);
                    //Debug.Log("✅ Dealt " + damage + " to Home");
                }
            }
            else if (hitCollider.CompareTag("Player") || hitCollider.CompareTag("Enemy"))
            {
                var healthComponent = hitCollider.GetComponent<Health>() ?? hitCollider.GetComponentInParent<Health>();

                if (healthComponent != null)
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    float damage = Mathf.Max(0, maxDamage * (1 - distance / explosionRadius));
                    healthComponent.ApplyDamage(damage);
                    //Debug.Log("✅ Dealt " + damage + " to " + hitCollider.name);
                }
            }
        }

        Destroy(gameObject);
    }


}