using System.Collections;
using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float rotationSpeed = 5f;
    private Rigidbody rb;
    public float launchForce = 20f;
    public float upwardForce = 8f;
    private float currentZRotation;
    private bool zInitialized = false;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        //Apply an angled force at launch (forward + upward)
        // Vector3 forceDirection = transform.forward * launchForce + transform.up * upwardForce;
        // rb.AddForce(forceDirection, ForceMode.VelocityChange);
    }

    [System.Obsolete]
    void FixedUpdate()
    {

        // if (rb != null && rb.velocity.magnitude > 0.1f)
        // {
        //     // Rotate to match velocity
        //     transform.rotation = Quaternion.LookRotation(rb.velocity) * Quaternion.Euler(0, 90, 0);
        // }
    }




}




