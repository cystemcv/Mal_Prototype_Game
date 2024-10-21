using System.Collections;
using UnityEngine;
using UnityEngine.U2D; // Required for PixelPerfectCamera

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float panSpeed = 0.8f; // Speed of panning
    public Vector2 panLimit = new Vector2(50, 50); // Limits for panning the camera
    public float scrollSpeed = 1f; // Adjusted zoom speed
    public float minZoom = 5f;     // Minimum orthographic size
    public float maxZoom = 20f;    // Maximum orthographic size
    public float smoothSpeed = 0.125f; // Speed of smoothing (adjustable)

    private Vector3 lastMousePosition;
    private Camera cam;
    private PixelPerfectCamera pixelPerfectCamera;

    void Start()
    {
        cam = GetComponent<Camera>(); // Get the Camera component
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

        GameObject target;
        if (CombatManager.Instance.planetClicked != null)
        {
            target = CombatManager.Instance.planetClicked;
        }
        else
        {
            target = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("DungeonParent").gameObject;
        }

        StartCoroutine(CenterCameraOnObject(target)); // Start the coroutine for smooth centering
    }

    public void GoToCenterOfGalaxy()
    {
        GameObject target;
        target = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("DungeonParent").gameObject;
        StartCoroutine(CenterCameraOnObject(target)); // Start the coroutine for smooth centering
    }

    private IEnumerator CenterCameraOnObject(GameObject target)
    {
        // Center the camera on the target GameObject smoothly
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.z = -10; // Ensure the Z position is set for 2D

            // Smoothly move the camera to the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
                yield return null; // Wait for the next frame
            }

            // Ensure the camera ends exactly at the target position
            transform.position = targetPosition;
        }
    }

    void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to start panning
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Continue panning while right-click is held
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Convert mouse movement to world space
            Vector3 movement = new Vector3(-mouseDelta.x * panSpeed, -mouseDelta.y * panSpeed, 0);
            movement = cam.ScreenToWorldPoint(movement) - cam.ScreenToWorldPoint(Vector3.zero);

            // Apply movement directly to the camera
            transform.position += movement;

            // Clamp camera position
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Clamp(newPosition.x, -panLimit.x, panLimit.x);
            newPosition.y = Mathf.Clamp(newPosition.y, -panLimit.y, panLimit.y);
            newPosition.z = -10; // Ensure Z stays at -10

            transform.position = newPosition;

            lastMousePosition = Input.mousePosition; // Update the last mouse position
        }
    }
}
