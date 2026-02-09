using UnityEngine;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class RTSCameraController : MonoBehaviour
{
    public static CinemachineCamera cinemachineCamera;
    public float moveSpeed = 100f;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 10f;

    public float minY = 15f;
    public float maxY = 90f;

    public float edgeScrollThreshold = 10f;

    private Vector3 lastMousePosition;
    private Terrain terrain;
    private Vector3 terrainMinBounds;
    private Vector3 terrainMaxBounds;

    [Header("Mobile Controls")]
    public FixedJoystick moveJoystick;    // Assign in Inspector
    public FixedJoystick rotationJoystick; // Optional second joystick for rotation

    private float lastPinchDistance; // Used for pinch-to-zoom

    void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
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
        if (cinemachineCamera.Priority >= 10)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            HandleMovementPC();
            HandleRotationPC();
            HandleZoomPC();
#elif UNITY_ANDROID || UNITY_IOS
            HandleMovementMobile();
            HandleRotationMobile();
            HandleZoomMobile(); // 👈 Pinch-to-zoom
#endif
        }
    }

    // -------------------- PC Controls --------------------
    void HandleMovementPC()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.D)) move += transform.right;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;

        Vector3 mousePos = Input.mousePosition;
        if (mousePos.x >= Screen.width - edgeScrollThreshold) move += transform.right;
        if (mousePos.x <= edgeScrollThreshold) move -= transform.right;
        if (mousePos.y >= Screen.height - edgeScrollThreshold) move += transform.forward;
        if (mousePos.y <= edgeScrollThreshold) move -= transform.forward;

        MoveCamera(move);
    }

    void HandleRotationPC()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationY = delta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationY, 0f, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }

    void HandleZoomPC()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(-scroll * zoomSpeed);
    }

    // -------------------- Mobile Controls --------------------
    void HandleMovementMobile()
    {
        if (moveJoystick == null) return;

        Vector3 move = transform.forward * moveJoystick.Vertical +
                       transform.right * moveJoystick.Horizontal;

        MoveCamera(move);
    }

    void HandleRotationMobile()
    {
        if (rotationJoystick == null) return;

        float rotationY = rotationJoystick.Horizontal * rotationSpeed;
        transform.Rotate(0f, rotationY * Time.deltaTime * 10, 0f, Space.World);
    }

    // new Input System

    void HandleZoomMobile()
    {
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
            return;

        var touch0 = Touchscreen.current.touches[0];
        var touch1 = Touchscreen.current.touches[1];

        if (!touch0.isInProgress || !touch1.isInProgress)
            return;

        // Block zoom if either finger is over UI
        if (IsTouchOverUI(touch0.position.ReadValue()) ||
            IsTouchOverUI(touch1.position.ReadValue()))
        {
            return;
        }

        // Get positions
        Vector2 pos0 = touch0.position.ReadValue();
        Vector2 pos1 = touch1.position.ReadValue();
        Vector2 prevPos0 = pos0 - touch0.delta.ReadValue();
        Vector2 prevPos1 = pos1 - touch1.delta.ReadValue();

        float prevMagnitude = (prevPos0 - prevPos1).magnitude;
        float currentMagnitude = (pos0 - pos1).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        ZoomCamera(-difference * 0.05f * zoomSpeed);
    }

    bool IsTouchOverUI(Vector2 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }



    // -------------------- Helpers --------------------
    void MoveCamera(Vector3 move)
    {
        move.y = 0;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, terrainMinBounds.x, terrainMaxBounds.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, terrainMinBounds.z, terrainMaxBounds.z);
        transform.position = clampedPosition;
    }

    void ZoomCamera(float delta)
    {
        Vector3 pos = transform.position;
        pos.y += delta;
        pos.y = Mathf.Clamp(pos.y, minY + 5, maxY);
        transform.position = pos;
    }
}
