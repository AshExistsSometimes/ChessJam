using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera cam;

    [Header("Orbit Angles")]
    public float[] horizontalAngles = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };

    [Range(0, 7)]
    public int angleIndex = 0;

    [Range(10f, 80f)]
    public float verticalAngle = 45f; // angle looking down in degrees

    [Header("Distance and Height")]

    [Range(1f, 20f)]
    public float distance = 10f;

    [Range(0f, 10f)]
    public float height = 3f;

    [Header("Rotation Lerp Settings")]
    public float rotationLerpSpeed = 6f;

    [Header("Follow Lerp Settings")]
    public float followSmoothTime = 0.15f;
    public float followLerpDuration = 0.5f;

    [Header("Zoom Settings")]
    [Range(1f, 10f)] public float minZoom = 2f;
    [Range(1f, 10f)] public float maxZoom = 5f;
    public float zoomSpeed = 2f;
    public float defaultZoom = 3.5f;

    // Internal state
    private float currentAngle;
    private float targetAngle;

    private Vector3 currentVelocity = Vector3.zero;

    private bool isPlayerMoving = false;
    private Vector3 lastPlayerPos;
    private Vector3 lerpStartPos;
    private Vector3 lerpTargetPos;
    private float lerpTimer = 0f;

    private Vector3 targetPosition;

    private float targetZoom;


    void Start()
    {
        if (player == null) Debug.LogError("Player transform not assigned!");
        if (cam == null) cam = Camera.main;

        currentAngle = horizontalAngles[angleIndex];
        targetAngle = currentAngle;

        lastPlayerPos = player.position;
        lerpTargetPos = lastPlayerPos + Vector3.up * height;
        lerpStartPos = lerpTargetPos;
        targetPosition = lerpTargetPos;

        targetZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);
        cam.orthographicSize = targetZoom;

        UpdateCameraPosition(immediate: true);
    }

    void Update()
    {
        HandleInput();

        HandleZoomInput();

        HandleAngleLerp();

        HandlePlayerMovementFollow();

        UpdateCameraPosition();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (PauseMenuController.IsPaused) return;
            int nextIndex = (angleIndex + 1) % horizontalAngles.Length;
            targetAngle = horizontalAngles[nextIndex];
            angleIndex = nextIndex;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (PauseMenuController.IsPaused) return;
            int prevIndex = (angleIndex - 1 + horizontalAngles.Length) % horizontalAngles.Length;
            targetAngle = horizontalAngles[prevIndex];
            angleIndex = prevIndex;
        }
    }


    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            if (PauseMenuController.IsPaused) return;
            float direction = Mathf.Sign(-scroll);
            targetZoom += direction * 0.5f;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Smoothly lerp to the target zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10f);
    }


    void HandleAngleLerp()
    {
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationLerpSpeed);
    }

    void HandlePlayerMovementFollow()
    {
        // Detect player movement by comparing position each frame
        if (Vector3.Distance(player.position, lastPlayerPos) > 0.01f)
        {
            if (!isPlayerMoving)
            {
                // Player started moving
                isPlayerMoving = true;
                lerpTimer = 0f; // cancel any lerp in progress
            }
            lastPlayerPos = player.position;
        }
        else
        {
            if (isPlayerMoving)
            {
                // Player stopped moving, start lerp from current to new position
                isPlayerMoving = false;
                lerpStartPos = targetPosition;
                lerpTargetPos = player.position + Vector3.up * height;
                lerpTimer = 0f;
            }
        }

        if (!isPlayerMoving && lerpTimer < followLerpDuration)
        {
            lerpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / followLerpDuration);
            targetPosition = Vector3.Lerp(lerpStartPos, lerpTargetPos, t);
        }
        else if (isPlayerMoving)
        {
            // Follow player directly while moving
            targetPosition = player.position + Vector3.up * height;
        }
    }

    void UpdateCameraPosition(bool immediate = false)
    {
        Vector3 desiredCamPos = CalculateCameraPosition(targetPosition);

        if (immediate)
        {
            cam.transform.position = desiredCamPos;
            cam.transform.LookAt(player.position + Vector3.up * height);
        }
        else
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, desiredCamPos, ref currentVelocity, followSmoothTime);
            cam.transform.LookAt(player.position + Vector3.up * height);
        }
    }

    Vector3 CalculateCameraPosition(Vector3 pivotPosition)
    {
        float horizontalRad = Mathf.Deg2Rad * currentAngle;
        float verticalRad = Mathf.Deg2Rad * verticalAngle;

        float x = distance * Mathf.Cos(verticalRad) * Mathf.Sin(horizontalRad);
        float y = distance * Mathf.Sin(verticalRad);
        float z = distance * Mathf.Cos(verticalRad) * Mathf.Cos(horizontalRad);

        return pivotPosition + new Vector3(x, y, z);
    }
}
