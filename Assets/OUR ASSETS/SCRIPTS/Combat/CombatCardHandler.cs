using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCardHandler : MonoBehaviour
{

    public static CombatCardHandler Instance;

    [Header("CAMERA")]
    public Camera mainCamera;

    [Header("VARIABLES THAT ARE ASSIGNED DYNAMICALLY")]
    public RectTransform targetUIElement;
    public GameObject targetClicked;

    [Header("VISUAL PREFABS")]
    public GameObject discardEffect;
    public GameObject banishEffect;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }


    public void Update()
    {

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.TARGET)
        {

            // Check if the target UI element is available
            if (targetUIElement != null)
            {
                // Get the position of the target UI element in screen space
                //Vector3 targetScreenPosition = targetUIElement.Find("LineRendererStart").position;
                Vector3 targetScreenPosition = RectTransformUtility.WorldToScreenPoint(CombatCardHandler.Instance.mainCamera, targetUIElement.Find("LineRendererStart").position);


                // Convert the screen space position to world space
                Vector3 targetPosition = Camera.main.ScreenToWorldPoint(targetScreenPosition);
                targetPosition.z = 0f; // Ensure that the z-coordinate is set to 0

                // Get the position of the mouse in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f; // Ensure that the z-coordinate is set to 0

                // Calculate the control point for the Bezier curve
                Vector3 controlPoint = (targetPosition + mousePosition) / 2;
                controlPoint.y += Mathf.Abs(mousePosition.x - targetPosition.x) / 2; // Adjust the curve height


                // Generate the points for the Bezier curve
                int segmentCount = 20;
                Vector3[] curvePoints = new Vector3[segmentCount + 1];
                for (int i = 0; i <= segmentCount; i++)
                {
                    float t = i / (float)segmentCount;
                    curvePoints[i] = CalculateQuadraticBezierPoint(t, targetPosition, controlPoint, mousePosition);
                }

                // Set the positions for the line renderer
                UI_Combat.Instance.cardLineRenderer.positionCount = curvePoints.Length;
                UI_Combat.Instance.cardLineRenderer.SetPositions(curvePoints);

                // Position the arrowhead at the end of the curve
                UI_Combat.Instance.cardLineRendererArrow.transform.position = curvePoints[curvePoints.Length - 1];
                // Rotate the arrowhead to face the direction of the line
                Vector3 direction = curvePoints[curvePoints.Length - 1] - curvePoints[curvePoints.Length - 2];
                UI_Combat.Instance.cardLineRendererArrow.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);


                //remove line renderer
                UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(true);
                UI_Combat.Instance.cardLineRendererArrow.SetActive(true);

                // Update the positions of the line renderer
                //lineRenderer.SetPosition(0, targetPosition);
                //lineRenderer.SetPosition(1, mousePosition);
            }
            CheckHitTarget();
            CheckClickTarget();
        }

    }

    public void CheckHitTarget()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        string cardTag = "";
        if (targetUIElement.gameObject.GetComponent<CardScript>().scriptableCard.targetEnemy == true)
        {
            cardTag = "Enemy";
        }
        else
        {
            cardTag = "Player";
        }

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one
            if (hit.collider.gameObject.tag == "Enemy" && cardTag == "Enemy")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue));

            }
            else if (hit.collider.gameObject.tag == "Player" && cardTag == "Player")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorLightBlue));

            }
            else
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));
            }

        }
        else
        {
            if (cardTag == "Player")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorGreen));
            }
            else if (cardTag == "Enemy")
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed));
            }
            else
            {
                ChangeLineAndArrowColor(SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite));
            }
        }

    }

    public void ChangeLineAndArrowColor(Color color)
    {
        UI_Combat.Instance.cardLineRendererArrow.GetComponent<SpriteRenderer>().color = color;
        UI_Combat.Instance.cardLineRenderer.startColor = color;
        UI_Combat.Instance.cardLineRenderer.endColor = color;
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }

    public void CheckClickTarget()
    {

        if (Input.GetMouseButtonDown(0))
        {
            HitTarget();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HitTarget();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;
        }
    }


    public void HitTarget()
    {
        // Cast a ray from mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // Check if the ray intersects with any colliders
        if (hit.collider != null)
        {
            //check if the target is the required one or it is not dead
            if (hit.collider.gameObject.tag != "Enemy" && hit.collider.gameObject.tag != "Player" || (hit.collider.gameObject.GetComponent<EntityClass>().entityMode == SystemManager.EntityMode.DEAD))
            {
                return;
            }

            string cardTag = "";
            if (targetUIElement.gameObject.GetComponent<CardScript>().scriptableCard.targetEnemy == true)
            {
                cardTag = "Enemy";
            }
            else
            {
                cardTag = "Player";
            }

            if (hit.collider.gameObject.tag != cardTag)
            {
                return;
            }

            // Handle the click
            //Debug.Log("Mouse clicked on: " + hit.collider.gameObject.name);
            // Add your click handling code here
            targetClicked = hit.collider.gameObject;

            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);
            UI_Combat.Instance.cardLineRendererArrow.SetActive(false);

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

            //do the effects 
            DeckManager.Instance.PlayCard(targetUIElement.gameObject.GetComponent<CardScript>());
            //return everything where it was
            HandManager.Instance.SetHandCards();




        }
    }
}