using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    public Camera mainCamera; // Camera to use for calculations
    public GameObject target; // The object you're tracking
    public RectTransform offscreenIndicator; // The UI indicator (e.g., an arrow icon)
    public float edgePadding = 10f; // Padding from the screen edges

    public TMP_Text distanceText; // UI text element to show the distance

    private void Start()
    {
        target = GameObject.Find("SYSTEM").transform.Find("DUNGEON MANAGER").Find("DungeonParent").gameObject;
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(target.transform.position);

        bool isOffscreen = screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;

        if (isOffscreen)
        {
            // The object is off-screen, calculate the direction
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.transform.position);
            Vector3 clampedScreenPos = screenPos;

            // Clamp the position to the screen bounds, minus the padding
            clampedScreenPos.x = Mathf.Clamp(clampedScreenPos.x, edgePadding, Screen.width - edgePadding);
            clampedScreenPos.y = Mathf.Clamp(clampedScreenPos.y, edgePadding, Screen.height - edgePadding);

            // Show the indicator at the clamped position
            offscreenIndicator.position = clampedScreenPos;
            offscreenIndicator.gameObject.SetActive(true);

            // Rotate the indicator to point towards the target
            Vector3 direction = target.transform.position - mainCamera.transform.position;
            float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            offscreenIndicator.rotation = Quaternion.Euler(0, 0, angle - 90); // Adjust for the correct rotation

            // Calculate the distance and update the distance text
            float distance = Vector3.Distance(mainCamera.transform.position, target.transform.position);
            distanceText.text = $"{distance:F1}";
        }
        else
        {
            // The object is on-screen, hide the indicator
            offscreenIndicator.gameObject.SetActive(false);
        }
    }
}
