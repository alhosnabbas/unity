using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmerchDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject rocketLauncherPrefab;
    public LayerMask terrainLayer; // Assign "Terrain" layer in Inspector

    private GameObject draggingRocketInstance;
    private Camera mainCamera;
    private Vector3 originalScale;
    private float objectHeight = 0f;
    private LauncherManager launcherManager;

    private static TextMeshProUGUI errorText;

    void Start()
    {
        mainCamera = Camera.main;
        // Terrain terrain = Terrain.activeTerrain;
        launcherManager = FindFirstObjectByType<LauncherManager>();

        if (errorText == null)
        {
            GameObject errorText1 = GameObject.Find("Error Message");
            if (errorText1 != null)
                errorText = errorText1.GetComponent<TextMeshProUGUI>();
            errorText.text = ""; // Initialize error text to empty
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rocketLauncherPrefab == null) return;

        draggingRocketInstance = Instantiate(rocketLauncherPrefab);

        // Save its original prefab scale
        originalScale = draggingRocketInstance.transform.localScale;

        // Estimate height from bounds so we can sit on ground
        Collider col = draggingRocketInstance.GetComponent<Collider>();
        if (col != null)
            objectHeight = col.bounds.extents.y; // half height of object
        else
            objectHeight = 0.5f; // fallback

        Rigidbody rb = draggingRocketInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Place only if terrain is hit
        if (!UpdateDragPosition(eventData))
        {
            Destroy(draggingRocketInstance);
            draggingRocketInstance = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingRocketInstance == null) return;
        UpdateDragPosition(eventData);
    }

    private bool UpdateDragPosition(PointerEventData eventData)
    {
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, terrainLayer))
        {
            if (draggingRocketInstance.name.Contains("V2Rocket(Clone)"))
            {
                draggingRocketInstance.transform.position = hit.point + (Vector3.up * objectHeight) + new Vector3(0, 15, 0);
            }
            else
            {

                draggingRocketInstance.transform.position = hit.point + Vector3.up * objectHeight;
            }
            draggingRocketInstance.transform.localScale = originalScale;
            return true;
        }

        return false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingRocketInstance == null) return;

        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        // Only raycast against terrain+water (your terrainLayer mask)
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, terrainLayer))
        {
            string hitLayer = LayerMask.LayerToName(hit.collider.gameObject.layer);
            Debug.Log($"Ray hit: {hit.collider.gameObject.name} on layer {hitLayer}");

            // ❌ Reject water
            if (hitLayer == "Water")
            {
                // Debug.Log("Invalid placement: cannot place launcher on water.");
                errorText.text = "Invalid placement: cannot place launcher on water.";
                Invoke("ClearError", 3f);
                Destroy(draggingRocketInstance);
                draggingRocketInstance = null;
                return;
            }

            // ✅ Place on terrain
            if (!launcherManager.isMaxReached)
            {
                Debug.Log(draggingRocketInstance.name);

                if (draggingRocketInstance.name.Contains("V2Rocket(Clone)"))
                {
                    draggingRocketInstance.transform.position = hit.point + (Vector3.up * objectHeight) + new Vector3(0, 15, 0);
                }
                else
                {

                    draggingRocketInstance.transform.position = hit.point + Vector3.up * objectHeight;
                }
                draggingRocketInstance.tag = "Player";

                Rigidbody rb = draggingRocketInstance.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                }

                launcherManager.AddLauncher(draggingRocketInstance.GetComponent<Launcher>(), true);
            }
            else
            {
                Destroy(draggingRocketInstance);
            }
        }
        else
        {
            // No terrain/water hit → cancel placement
            Destroy(draggingRocketInstance);
        }

        draggingRocketInstance = null;
    }
    private void ClearError()
    {
        errorText.text = "";
    }
}
