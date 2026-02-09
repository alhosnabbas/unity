using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class TochkaRocket : MonoBehaviour
{
    private Rigidbody rb;
    public ParticleSystem explosionEffect; // Assign in Inspector
    public GameObject explosionPrefab; // Assign in Inspector                                       
    public float explosionRadius = 30f;
    public float maxDamage = 500f;
    private Health healthComponent; // For Player or Enemy
    private HealthAllObjects homeHealthComponent; // For Home
    LineRenderer velocityLine;
    void Start()
    {
        delay();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        // velocityLine = transform.AddComponent<LineRenderer>();
        // velocityLine.positionCount = 2;
        // velocityLine.startWidth = 0.1f;
        // velocityLine.endWidth = 0.1f;
        // velocityLine.material = new Material(Shader.Find("Sprites/Default")); // simple shader
        // velocityLine.startColor = Color.green;
        // velocityLine.endColor = Color.green;

    }

    [System.Obsolete]
    void Update()
    {
        // if (velocityLine != null)
        // {

        //     velocityLine.SetPosition(0, transform.position);
        //     velocityLine.SetPosition(1, transform.position + rb.velocity.normalized * 20f); // adjust length
        // }
        if (rb.transform.position.y < 0)
        {
            OutOffBounds(); // Destroy the rocket if it falls below y = 0
        }
        if (rb != null && rb.linearVelocity.magnitude > 0.1f)
        {
            // Rotate to match velocity
            //rb.isKinematic = false;
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(-90, 0, -90);
        }
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);

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

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
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

        // Debug.Log("TochkaRocket collided with " + collision.gameObject.name);

        GameObject sound = Instantiate(explosionPrefab, transform.position, Quaternion.identity);





        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, explosion.main.duration); // Destroy the effect after it finishes playing
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
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
                        // Debug.Log("Damage dealt: " + damage + " to " + hitCollider.name);
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
}