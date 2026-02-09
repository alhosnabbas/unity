using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System;

public class V2 : Launcher
{
    private float launchSpeed = 100f;
    private float maxRange = 700f;
    public float pitchOverDelay = 1.5f;
    public float pitchOverDuration = 2.0f;
    private float targetSpread = 50f;
    public AudioSource rocketSound;
    public AudioClip rocketLaunchClip;
    public TextMeshProUGUI errorText;

    private Vector3 targetPosition;
    private Rigidbody rb;
    private bool isLaunched = false;
    private bool isPitchingOver = false;
    LineRenderer velocityLine;
    public ParticleSystem explosionEffect; // Assign in Inspector
    public GameObject explosionPrefab; // Assign in Inspector                                       
    public float explosionRadius = 50f;
    public float maxDamage = 800f;
    private Health healthComponent; // For Player or Enemy
    private HealthAllObjects homeHealthComponent; // For Home

    private PlayerTurn playerTurn;

    private LauncherManager launcherManager;
    [System.Obsolete]
    void Start()
    {
        isLaunched = false;
        // rb = GetComponent<Rigidbody>();
        //rb.useGravity = true;
        //rb.isKinematic = true;
        //rb.useGravity = true;
        if (errorText == null)
        {
            GameObject errorText1 = GameObject.Find("Error Message");
            if (errorText1 != null)
                errorText = errorText1.GetComponent<TextMeshProUGUI>();
            errorText.text = ""; // Initialize error text to empty
        }
        //rb = GetComponent<Rigidbody>();
        //rb.isKinematic = true;
        launcherManager = FindFirstObjectByType<LauncherManager>();
        playerTurn = FindFirstObjectByType<PlayerTurn>();
    }

    public void SetOwnership(bool isPlayer)
    {
        isPlayerOwned = isPlayer;

        // if (flag != null)
        // {
        //     Renderer flagRenderer = flag.GetComponent<Renderer>();
        //     if (flagRenderer != null)
        //     {
        //         flagRenderer.material.color = !isPlayer ? newColor : Color.blue;
        //     }
        // }
    }
    public override IEnumerator Fire(Vector3 position, float someValue, System.Action onComplete)
    {
        //  Debug.Log("Smerch fired!");
        // Put your Smerch rocket firing code here
        // Debug.Log("position" + position);
        // targetPosition = position;
        // Debug.Log("SmerchLauncher firing at: " + targetPosition);
        // for (int i = 0; i < 12; i++)
        // {

        StartCoroutine(LaunchRocket());
        if (onComplete != null) onComplete.Invoke();
        yield return new WaitForSeconds(1f);
        // }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //StartCoroutine(WaitAndInitiateRocket());
        }

        if (velocityLine != null)
        {

            // velocityLine.SetPosition(0, transform.position);
            // velocityLine.SetPosition(1, transform.position + rb.velocity.normalized * 20f); // adjust length
        }
    }

    [System.Obsolete]
    private IEnumerator WaitAndInitiateRocket()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(InitiateRocket(() => { }));
    }

    [System.Obsolete]
    public IEnumerator InitiateRocket(System.Action onComplete)
    {
        inputField inputField = FindObjectOfType<inputField>();
        if (inputField == null) yield break;

        string textX = inputField.inputFieldX.text;
        string textY = inputField.inputFieldY.text;

        if (string.IsNullOrEmpty(textX) || string.IsNullOrEmpty(textY))
        {
            errorText.text = "Enter X and Y target.";
            errorText.color = Color.red;
            Invoke("ClearError", 3f);
            onComplete?.Invoke();
            yield break;
        }

        targetPosition = new Vector3(float.Parse(textX), 0f, float.Parse(textY));

        if ((targetPosition - transform.position).magnitude > maxRange)
        {
            errorText.text = "Target is out of range!";
            errorText.color = Color.red;
            Invoke("ClearError", 3f);
            onComplete?.Invoke();
            yield break;
        }

        StartCoroutine(LaunchRocket());
        onComplete?.Invoke();
    }

    void ClearError() => errorText.text = "";

    [System.Obsolete]
    IEnumerator LaunchRocket()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = Vector3.up * launchSpeed;
        isLaunched = true;
        rocketSound?.PlayOneShot(rocketLaunchClip);

        yield return new WaitForSeconds(pitchOverDelay);
        StartCoroutine(PitchOverToTarget());
        yield return new WaitForSeconds(0.5f); // Small delay before launch
        isLaunched = true;
        if (playerTurn.GetCurrentPlayer() == 2)
        {
            playerTurn.AddCPUShot();
        }
        else
        {
            playerTurn.AddShot();
        }

    }

    [System.Obsolete]
    IEnumerator PitchOverToTarget()
    {
        isPitchingOver = true;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 horizontalDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z).normalized;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(horizontalDirection);

        float elapsed = 0f;
        while (elapsed < pitchOverDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / pitchOverDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;

        // Once pitched over, stop applying rotation and let physics take over
        ApplyTrajectoryVelocity();
    }

    [System.Obsolete]
    void ApplyTrajectoryVelocity()
    {
        Vector3 randomizedTarget = targetPosition;
        randomizedTarget.x += UnityEngine.Random.Range(-targetSpread, targetSpread);
        randomizedTarget.z += UnityEngine.Random.Range(-targetSpread, targetSpread);
        Vector3 displacement = randomizedTarget - transform.position;
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        float dx = horizontalDisplacement.magnitude;
        float dy = displacement.y;

        float angleRad;
        if (!CalculateLaunchAngle(displacement, gravity, launchSpeed, out angleRad))
        {
            // Debug.LogError("Cannot reach target with given speed.");
            return;
        }

        Vector3 dirXZ = horizontalDisplacement.normalized;
        Vector3 finalVelocity = dirXZ * launchSpeed * Mathf.Cos(angleRad) + Vector3.up * launchSpeed * Mathf.Sin(angleRad);

        rb.velocity = finalVelocity;
    }

    bool CalculateLaunchAngle(Vector3 displacement, float gravity, float speed, out float angleRad)
    {
        float dx = new Vector2(displacement.x, displacement.z).magnitude;
        float dy = displacement.y;
        float v2 = speed * speed;

        float discriminant = v2 * v2 - gravity * (gravity * dx * dx + 2 * dy * v2);
        if (discriminant < 0)
        {
            angleRad = 0;
            return false;
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float lowAngle = Mathf.Atan((v2 - sqrtDisc) / (gravity * dx));
        float highAngle = Mathf.Atan((v2 + sqrtDisc) / (gravity * dx));

        angleRad = (lowAngle * Mathf.Rad2Deg < 30f) ? highAngle : lowAngle;
        return true;
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        // if (rb.velocity.sqrMagnitude > 1f)
        //     rb.rotation = Quaternion.LookRotation(rb.velocity.normalized) * Quaternion.Euler(90, 0, 0);
    }


    [System.Obsolete]
    void OnCollisionEnter(Collision collision)
    {

        if (isLaunched)
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
                            //Debug.Log("Damage dealt: " + damage + " to " + hitCollider.name);
                        }
                    }

                }

            }

            Destroy(gameObject); // Destroy the rocket after explosion
        }

    }
}
