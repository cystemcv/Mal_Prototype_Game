using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Panning Settings")]
    public float panSpeed = 20f; // Speed of the camera movement
    public Vector2 minBounds; // Minimum x and y values for camera position
    public Vector2 maxBounds; // Maximum x and y values for camera position
    public float panBorderThickness = 10f; // Distance from the edge of the screen where panning starts

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float minZoom = 5f; // Minimum orthographic size
    public float maxZoom = 20f; // Maximum orthographic size

    [Header("Cursor Settings")]
    public Texture2D defaultCursor; // Default cursor texture
    public Texture2D zoomingCursor; // Zooming cursor texture
    public Vector2 cursorHotspot = Vector2.zero; // Cursor hotspot

    [Header("Smoothing Settings")]
    public float smoothSpeed = 0.125f; // Speed of the smooth movement
    public bool enableSmoothing = true; // Toggle smoothing on/off

    private Vector3 targetPosition;
    private float targetZoom;

    private enum CursorState { Default, Zooming }
    private CursorState currentCursorState = CursorState.Default;

    void Start()
    {
        targetPosition = transform.position;
        targetZoom = Camera.main.orthographicSize;

        // Ensure the cursor is visible and set the default cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetCursor(CursorState.Default);
    }

    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleCursorState();
        ApplyCameraMovement();
    }

    /// <summary>
    /// Handles camera panning by moving the mouse close to the screen edges.
    /// </summary>
    private void HandlePanning()
    {
        Vector3 move = Vector3.zero;

        // Check if the mouse is close to the screen edges and move the camera accordingly
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            move.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            move.x -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            move.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            move.y -= panSpeed * Time.deltaTime;
        }

        targetPosition = transform.position + move;

        // Clamp the target position within the defined bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    /// <summary>
    /// Handles camera zooming with the mouse wheel.
    /// </summary>
    private void HandleZooming()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0.0f)
        {
            targetZoom -= scrollData * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            SetCursor(CursorState.Zooming);
        }
    }

    /// <summary>
    /// Handles the cursor state based on user interactions.
    /// </summary>
    private void HandleCursorState()
    {
        // Reset zooming state if no zoom input is detected
        if (Input.GetAxis("Mouse ScrollWheel") == 0)
        {
            SetCursor(CursorState.Default);
        }
    }

    /// <summary>
    /// Applies the calculated target position and zoom to the camera with optional smoothing.
    /// </summary>
    private void ApplyCameraMovement()
    {
        if (enableSmoothing)
        {
            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Smoothly zoom the camera
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, smoothSpeed);
        }
        else
        {
            // Directly set the camera position and zoom without smoothing
            transform.position = targetPosition;
            Camera.main.orthographicSize = targetZoom;
        }
    }

    /// <summary>
    /// Sets the cursor based on the current state.
    /// </summary>
    /// <param name="state">The desired cursor state.</param>
    private void SetCursor(CursorState state)
    {
        if (currentCursorState == state) return;

        switch (state)
        {
            case CursorState.Default:
                Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                break;
            case CursorState.Zooming:
                Cursor.SetCursor(zoomingCursor, cursorHotspot, CursorMode.Auto);
                break;
        }
        currentCursorState = state;
    }

    /// <summary>
    /// Optional: Visualize the bounds in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 bottomLeft = new Vector3(minBounds.x, minBounds.y, transform.position.z);
        Vector3 topLeft = new Vector3(minBounds.x, maxBounds.y, transform.position.z);
        Vector3 topRight = new Vector3(maxBounds.x, maxBounds.y, transform.position.z);
        Vector3 bottomRight = new Vector3(maxBounds.x, minBounds.y, transform.position.z);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
