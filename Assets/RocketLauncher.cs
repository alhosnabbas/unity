using UnityEngine;
using System.Collections;
using TMPro; // Ensure you have TextMeshPro package installed
using UnityEngine.UI;
using Unity.VisualScripting; // For UI components
public class RocketLauncher : Launcher
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

    private float targetSpread = 10f;

    // Update is used here for demonstration; press Space to launch the rocket.
    GameObject rocket;
    public AudioSource rocketSound;
    public AudioClip rocketLaunchClip;

    private static TextMeshProUGUI errorText;

    private float maxRange = 600; // Maximum range for the launcher

    public GameObject flag;
    private Color newColor = Color.red;
    private PlayerTurn playerTurn;

    private LauncherManager launcherManager;
    void Update()
    {

    }

    void Awake()
    {

        if (flag == null)
            flag = transform.Find("Flag")?.gameObject;

    }
    void Start()
    {

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
        //flag = transform.Find("Flag").gameObject;
        // Debug.Log(isPlayerOwned);
        // if (flag != null && isPlayerOwned)
        // {
        //     Renderer flagRenderer = flag.GetComponent<Renderer>();
        //     if (flagRenderer != null)
        //     {
        //         flagRenderer.material.color = newColor;
        //     }
        // }
        playerTurn = FindFirstObjectByType<PlayerTurn>();
        launcherManager = FindFirstObjectByType<LauncherManager>();
    }
    public override void SetOwnership(bool isPlayer)
    {
        Debug.Log(isPlayer + "2222222222222222222222222222222");
        isPlayerOwned = isPlayer;
        Debug.Log(isPlayerOwned);
        if (flag != null)
        {
            Renderer flagRenderer = flag.GetComponent<Renderer>();
            if (flagRenderer != null)
            {
                flagRenderer.material.color = !isPlayer ? newColor : Color.blue;
            }
        }
    }
    public override IEnumerator Fire(Vector3 position, float someValue, System.Action onComplete)
    {
        //  Debug.Log("Smerch fired!");
        // Put your Smerch rocket firing code here
        Debug.Log("position" + position);
        targetPosition = position;
        Debug.Log("SmerchLauncher firing at: " + targetPosition);
        for (int i = 0; i < 12; i++)
        {

            StartCoroutine(LaunchRocket());
            yield return new WaitForSeconds(1f);
        }
        if (onComplete != null) onComplete.Invoke();

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
            // Debug.Log("Player typed: " + textX + " " + textY);
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
                // Debug.Log("Target Position: " + targetPosition.x + " " + targetPosition.z);
                targetPosition.y = 20f; // Assuming the height is constant at 20
                Vector3 displacement = targetPosition - launchPoint.position;

                if (displacement.magnitude > maxRange)
                {

                    errorText.text = "Target is out of range!";
                    errorText.color = Color.red;
                    Invoke("ClearError", 3f);
                    onComplete.Invoke();
                    //yield break;

                }
                else
                {




                    for (int i = 0; i < 12; i++)
                    {

                        StartCoroutine(LaunchRocket());
                        yield return new WaitForSeconds(1f);
                    }
                    if (onComplete != null)
                        onComplete.Invoke();
                }


            }
        }

        if (playerTurn.GetCurrentPlayer() == 2)
        {
            playerTurn.AddCPUShot();
        }
        else
        {
            playerTurn.AddShot();
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
            //  Debug.LogError("Assign rocketPrefab and launchPoint.");
            yield break;
        }

        float fixedSpeed = 90f; // Set your desired launch speed

        if (!CalculateLaunchData(displacement, gravity, fixedSpeed, out float angleRad))
        {
            // Debug.LogError("Target unreachable with the given speed.");
            yield break; // No solution found
        }


        Vector3 directionXZ = displacementXZ.normalized;
        Vector3 velocity = directionXZ * fixedSpeed * Mathf.Cos(angleRad) + Vector3.up * fixedSpeed * Mathf.Sin(angleRad);
        StartCoroutine(RotateLauncherTowards(velocity));
        yield return new WaitForSeconds(4f);
        // Instantiate rocket
        rocket = Instantiate(rocketPrefab, launchPoint.position + new Vector3(0.2f, 0.5f, 1f), Quaternion.LookRotation(velocity) * Quaternion.Euler(0, 90, 0));

        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        rb.useGravity = true;

        rb.velocity = velocity;
        rb.rotation = Quaternion.LookRotation(velocity) * Quaternion.Euler(0, 90, 0);
        rocketSound.PlayOneShot(rocketLaunchClip);
        //Debug.DrawRay(launchPoint.position, velocity, Color.green, 200f);


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




    IEnumerator RotateLauncherTowards(Vector3 velocity)
    {
        Quaternion targetRotation = Quaternion.LookRotation(velocity);

        float elapsedTime = 0f;
        float duration = 10f;

        while (elapsedTime < duration)
        {
            launchPoint.rotation = Quaternion.Slerp(launchPoint.rotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        launchPoint.rotation = targetRotation;
    }

    public void SetErrorText(TextMeshProUGUI text)
    {
        errorText = text;
    }
}
