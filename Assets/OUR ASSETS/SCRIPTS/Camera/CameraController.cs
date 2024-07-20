using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Speed of the camera movement
    public float smoothSpeed = 0.125f; // Speed of the smooth movement
    public Vector2 minBounds; // Minimum x and y values for camera position
    public Vector2 maxBounds; // Maximum x and y values for camera position
    public Texture2D defaultCursor; // Default cursor texture
    public Texture2D panningCursor; // Panning cursor texture
    public Texture2D zoomingCursor; // Zooming cursor texture
    public Vector2 cursorHotspot = Vector2.zero; // Cursor hotspot, set to (0,0) by default

    public float zoomSpeed = 10f; // Speed of zooming in and out
    public float minZoom = 5f; // Minimum orthographic size or field of view
    public float maxZoom = 20f; // Maximum orthographic size or field of view

    private Vector3 initialMousePosition;
    private Vector3 targetPosition;
    private bool isPanning;
    private bool isZooming;
    private float targetZoom;

    private enum CursorState { Default, Panning, Zooming }
    private CursorState currentCursorState = CursorState.Default;

    public Vector3 savedCameraPosition;

    public bool cameraReset = false;

    void Start()
    {
        savedCameraPosition = new Vector3(0, 0, -10);
        targetPosition = transform.position;
        targetZoom = Camera.main.orthographicSize; // Use Camera.main.fieldOfView for 3D

        // Ensure the cursor is visible and set the default cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SetCursor(CursorState.Default);
    }

    void Update()
    {

        //no camera control except for dungeon mode
        if(SystemManager.Instance.systemMode != SystemManager.SystemModes.DUNGEON)
        {
            cameraReset = true;
            SystemManager.Instance.mainCamera.transform.position = new Vector3(0, 0, -10);
            return;
        }
        else 
        {

            if (cameraReset)
            {
               this.transform.position = savedCameraPosition;
                cameraReset = false;
            }

            savedCameraPosition = this.transform.position;
        }

        // Handle panning
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            initialMousePosition = Input.mousePosition;
            SetCursor(CursorState.Panning); // Change to panning cursor
        }

        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
            if (!isZooming)
            {
                SetCursor(CursorState.Default); // Change back to default cursor if not zooming
            }
        }

        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;
            Vector3 move = new Vector3(-mouseDelta.x, -mouseDelta.y, 0) * panSpeed * Time.deltaTime;

            targetPosition = transform.position + move;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

            initialMousePosition = Input.mousePosition;
        }

        // Smoothly move the camera towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // Handle zooming
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        if (scrollData != 0.0f)
        {
            isZooming = true;
            targetZoom -= scrollData * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            SetCursor(CursorState.Zooming); // Change to zooming cursor
        }

        // Smoothly zoom the camera
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, smoothSpeed); // Use Camera.main.fieldOfView for 3D

        // Check if any mouse button is pressed or mouse moved to reset cursor
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            isZooming = false;
            if (!isPanning)
            {
                SetCursor(CursorState.Default); // Change back to default cursor if not panning
            }
            else
            {
                SetCursor(CursorState.Panning); // Ensure cursor stays as panning cursor while panning
            }
        }
    }

    private void SetCursor(CursorState state)
    {
        if (currentCursorState == state) return;

        switch (state)
        {
            case CursorState.Default:
                Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
                break;
            case CursorState.Panning:
                Cursor.SetCursor(panningCursor, cursorHotspot, CursorMode.Auto);
                break;
            case CursorState.Zooming:
                Cursor.SetCursor(zoomingCursor, cursorHotspot, CursorMode.Auto);
                break;
        }
        currentCursorState = state;
    }

    // Optional: Visualize the bounds in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, 0), new Vector3(minBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, 0), new Vector3(maxBounds.x, maxBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, 0), new Vector3(maxBounds.x, minBounds.y, 0));
        Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, 0), new Vector3(minBounds.x, minBounds.y, 0));
    }
}
