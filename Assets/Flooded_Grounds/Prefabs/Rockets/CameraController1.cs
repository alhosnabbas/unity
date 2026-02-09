using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class RTSCameraController1 : MonoBehaviour
{

    public static CinemachineCamera cinemachinecamera;
    private float moveSpeed = 100f;
    private float rotationSpeed = 5f;
    private float zoomSpeed = 20f;

    private float minY = 15f;
    private float maxY = 90f;

    private float edgeScrollThreshold = 10f;
    private float edgeScrollSpeed = 10f;

    private Vector3 lastMousePosition;
    private Terrain terrain;
    private Vector3 terrainMinBounds;
    private Vector3 terrainMaxBounds;

    void Awake()
    {
        cinemachinecamera = GetComponent<CinemachineCamera>();
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            Vector3 pos = terrain.GetPosition();
            Vector3 size = terrain.terrainData.size;

            terrainMinBounds = new Vector3(pos.x, 0, pos.z);
            terrainMaxBounds = new Vector3(pos.x + size.x, 0, pos.z + size.z);
        }
        else
        {
            Debug.LogError("No active terrain found!");
        }
    }


    void Update()
    {
        if (cinemachinecamera.Priority >= 10)
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();

        }

    }

    void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        // WASD Movement
        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;

        // Screen edge movement
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - edgeScrollThreshold) move += transform.right;
        if (mousePos.x <= edgeScrollThreshold) move -= transform.right;
        if (mousePos.y >= Screen.height - edgeScrollThreshold) move += transform.forward;
        if (mousePos.y <= edgeScrollThreshold) move -= transform.forward;

        move.y = 0;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;

        // Clamp position within terrain boundaries
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, terrainMinBounds.x, terrainMaxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, terrainMinBounds.z, terrainMaxBounds.z);
        transform.position = clampedPosition;
    }


    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Right Mouse Button
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationY = delta.x * rotationSpeed * Time.deltaTime;

            transform.Rotate(0f, rotationY, 0f, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * zoomSpeed;
        pos.y = Mathf.Clamp(pos.y, minY + 5, maxY);
        transform.position = pos;
    }
}
