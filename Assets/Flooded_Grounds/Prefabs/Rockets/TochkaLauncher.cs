using UnityEngine;
using System.Collections;
using TMPro; // Ensure you have TextMeshPro package installed
using UnityEngine.UI;
// For UI components
public class TochkaLauncher : Launcher
{
    [Header("Rocket Setup")]
    // Drag your rocket prefab here (it must have a Rigidbody component)
    public GameObject rocketPrefab;
    // The transform that marks the launch point (usually a child of the launcher)
    public Transform launchPoint;

    [Header("Target Settings")]
    // User-specified target coordinates (assumed to be at the same height as launchPoint)
    private Vector3 targetPosition;

    [Header("Launch Settings")]
    // Launch angle in degrees (fixed at 45° for this example)

    private float targetSpread = 5f;

    // Update is used here for demonstration; press Space to launch the rocket.
    GameObject rocket;
    public AudioSource rocketSound;
    public AudioClip rocketLaunchClip;

    private static TextMeshProUGUI errorText;

    private float maxRange = 900; // Maximum range for the launcher
    LineRenderer lineRenderer;
    public int segments = 100; // Circle smoothness
    public float yOffset = 100f; // Lift a bit above ground

    PlayerTurn playerTurn;
    private LauncherManager launcherManager;
    [System.Obsolete]
    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.red);
        // Initialize the line

        if (rocket != null)
        {
            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            // velocityLine.SetPosition(0, rocket.transform.position);
            // velocityLine.SetPosition(1, rocket.transform.position + rb.velocity.normalized * 20f); // adjust length
        }
    }


    void Start()
    {
        rocket = Instantiate(rocketPrefab);
        Transform rocketMountPoint = transform.Find("Rocket");
        rocket.transform.SetParent(rocketMountPoint);
        rocket.transform.localPosition = Vector3.zero + new Vector3(-0.300000012f, 1.70000005f, 10.3000002f);
        rocket.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, -90, 0);
        playerTurn = FindFirstObjectByType<PlayerTurn>();

        launcherManager = FindFirstObjectByType<LauncherManager>();


        if (rocketSound == null)
        {
            rocketSound = GetComponent<AudioSource>();
        }
        if (errorText == null)
        {
            GameObject errorText1 = GameObject.Find("Error Message");
            if (errorText1 != null)
                errorText = errorText1.GetComponent<TextMeshProUGUI>();
            errorText.text = ""; // Initialize error text to empty
        }
    }

    public override IEnumerator Fire(Vector3 targetPos, float spread = 5f, System.Action onComplete = null)
    {
        if (rocketPrefab == null || launchPoint == null)
            yield break;
        targetPosition = targetPos;
        Debug.Log("TochkaLauncher firing at: " + targetPosition);
        StartCoroutine(LaunchRocket());
        if (onComplete != null) onComplete.Invoke();
        yield return new WaitForSeconds(3f);
        // Base direction to target
        // Vector3 direction = (targetPos - launchPoint.position).normalized;

        // // Add random spread (in degrees)
        // direction = Quaternion.Euler(
        //     Random.Range(-spread, spread),
        //     Random.Range(-spread, spread),
        //     0
        // ) * direction;

        // // Spawn rocket
        // GameObject rocket = Instantiate(rocketPrefab, launchPoint.position, Quaternion.LookRotation(direction));

        // // Add velocity/force
        // Rigidbody rb = rocket.GetComponent<Rigidbody>();
        // if (rb != null)
        // {
        //     float rocketSpeed = 50f; // tweak to your needs
        //     rb.velocity = direction * rocketSpeed;
        // }


    }


    void DrawCircle()
    {
        lineRenderer.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * maxRange;
            float z = Mathf.Sin(angle) * maxRange;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
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
    [System.Obsolete]
    public IEnumerator InitiateRocket(System.Action onComplete)
    {
        inputField inputField = FindObjectOfType<inputField>();
        if (inputField != null)
        {
            // Read the input values from the input fields
            string textX = inputField.inputFieldX.text;
            string textY = inputField.inputFieldY.text;
            //            Debug.Log("Player typed: " + textX + " " + textY);
            if (string.IsNullOrEmpty(textX))
            {
                errorText.text = "Please enter a valid X coordinate.";
                errorText.color = Color.red;
                Invoke("ClearError", 3f);
                onComplete.Invoke();

            }
            else if (string.IsNullOrEmpty(textY))
            {
                errorText.text = "Please enter a valid Y coordinate.";
                errorText.color = Color.red;
                Invoke("ClearError", 3f);
                onComplete.Invoke();
            }
            else
            {


                targetPosition.x = float.Parse(textX);
                targetPosition.z = float.Parse(textY);
                //Debug.Log("Target Position: " + targetPosition.x + " " + targetPosition.z);
                targetPosition.y = 20f; // Assuming the height is constant at 20
                Vector3 displacement = targetPosition - launchPoint.position;
                // Debug.Log(displacement.magnitude + " > " + maxRange);


                StartCoroutine(LaunchRocket());
                yield return new WaitForSeconds(3f);

                if (onComplete != null)
                    playerTurn.AddShot();
                onComplete.Invoke();



            }
        }



    }
    void ClearError()
    {
        errorText.text = "";
    }

    [System.Obsolete]
    public IEnumerator LaunchRocket()
    {
        Vector3 randomizedTarget = targetPosition;
        randomizedTarget.x += Random.Range(-targetSpread, targetSpread);
        randomizedTarget.z += Random.Range(-targetSpread, targetSpread);

        Vector3 displacement = randomizedTarget - launchPoint.position;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float gravity = Mathf.Abs(Physics.gravity.y);

        if (rocketPrefab == null || launchPoint == null)
        {
            // Debug.LogError("Assign rocketPrefab and launchPoint.");
            yield break;
        }

        float fixedSpeed = 80f; // Set your desired launch speed

        if (!CalculateLaunchData(displacement, gravity, fixedSpeed, out float angleRad))
        {
            errorText.text = "Target is out of range!";
            errorText.color = Color.red;
            Invoke("ClearError", 3f);
            //onComplete.Invoke();
            yield break; // No solution found
        }


        Vector3 directionXZ = displacementXZ.normalized;
        Vector3 velocity = directionXZ * fixedSpeed * Mathf.Cos(angleRad) + Vector3.up * fixedSpeed * Mathf.Sin(angleRad);
        //Debug.Log("Velocity: " + velocity);
        // Instantiate rocket
        if (rocket == null)
        {

            rocket = Instantiate(rocketPrefab);

            Transform rocketMountPoint = transform.Find("Rocket");
            rocket.transform.SetParent(rocketMountPoint);
            rocket.transform.localPosition = Vector3.zero + new Vector3(-0.300000012f, 1.70000005f, 10.3000002f);
            rocket.transform.localRotation = Quaternion.identity * Quaternion.Euler(0, -90, 0);
        }
        // Rigidbody trb = GetComponent<Rigidbody>();
        // trb.isKinematic = true;
        StartCoroutine(RotateLauncherTowards(velocity, rocket, directionXZ));

        yield return new WaitForSeconds(10f);
        Rigidbody trb = GetComponent<Rigidbody>();
        trb.isKinematic = false;
        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        rb.rotation = Quaternion.LookRotation(rb.linearVelocity) * Quaternion.Euler(-90, 0, -90);
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.velocity = velocity;
        //yield return new WaitForSeconds(1f);
        rocketSound.PlayOneShot(rocketLaunchClip);
        //Debug.DrawRay(launchPoint.position, velocity, Color.green, 200f);

        if (playerTurn.GetCurrentPlayer() == 2)
        {
            playerTurn.AddCPUShot();
        }
        else
        {
            playerTurn.AddShot();
        }
    }

    bool CalculateLaunchData(Vector3 displacement, float gravity, float speed, out float angleRad)
    {
        float dx = new Vector2(displacement.x, displacement.z).magnitude;
        float dy = displacement.y;

        float v2 = speed * speed;
        float discriminant = v2 * v2 - gravity * (gravity * dx * dx + 2 * dy * v2);

        if (discriminant < 0)
        {
            angleRad = 0;
            return false; // No solution
        }

        float sqrtDisc = Mathf.Sqrt(discriminant);
        float lowAngle = Mathf.Atan((v2 - sqrtDisc) / (gravity * dx));
        float highAngle = Mathf.Atan((v2 + sqrtDisc) / (gravity * dx));

        // You can choose lowAngle or highAngle. Here we use the lower one for a flatter trajectory.
        if (lowAngle * Mathf.Rad2Deg < 30f)
            angleRad = highAngle;
        else
            angleRad = lowAngle;

        // Debug.Log("Angle: " + angleRad * Mathf.Rad2Deg + " degrees" + displacement.magnitude);
        return true;
    }




    IEnumerator RotateLauncherTowards(Vector3 velocity, GameObject rocket, Vector3 directionXZ)
    {
        Quaternion launcherTargetRotation = Quaternion.LookRotation(directionXZ);
        Quaternion rocketStartRotation = rocket.transform.rotation;
        Quaternion rocketTargetRotation = Quaternion.LookRotation(velocity);
        // or add offset if needed

        float elapsedTime = 0f;
        float duration = 5f; // Shorter duration for rocket adjustment

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, launcherTargetRotation, elapsedTime / duration);
            //rocket.transform.rotation = Quaternion.Slerp(rocketStartRotation, transform.rotation * Quaternion.Euler(260, 0, 0), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotations are exact
        transform.rotation = launcherTargetRotation;

        yield return new WaitForSeconds(3f);

        elapsedTime = 0f;
        duration = 5f;
        while (elapsedTime < duration)
        {
            // transform.rotation = Quaternion.Slerp(transform.rotation, launcherTargetRotation, elapsedTime / duration);
            launchPoint.transform.rotation = Quaternion.Slerp(launchPoint.transform.rotation, rocketTargetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        launchPoint.transform.rotation = rocketTargetRotation;

        yield return new WaitForSeconds(0.1f);
        launchPoint.transform.rotation = launchPoint.transform.rotation * Quaternion.Euler(-launchPoint.transform.rotation.x, -launchPoint.transform.rotation.y, -launchPoint.transform.rotation.z);
    }



    public void SetErrorText(TextMeshProUGUI text)
    {
        errorText = text;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawLine(rocket.transform.position, rocket.transform.position + rocket.transform.forward * 5f);
    }

}
