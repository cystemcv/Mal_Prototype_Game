using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private float hoverScale = 1.2f;
    private float transitionTime = 0.1f;
    private float hoverHeight = 100f;

    public int sortOrder = 0;

    private Vector3 originalScale;

    private Vector3 saveRotation;


    private int index = -1; // Initialize the index to -1

    private GameObject childObjectVisual;

    public LTDescr scaleTween;
    public LTDescr moveTween;
    public LTDescr localMoveTween;

    //new variables that actually work
    public float originalPosX;
    public float originalPosY;

    public enum stateOfEvent { hover,exithover,clicked }

    public stateOfEvent currentEvent;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;


    private bool canActivate = false;
    private bool isDragging = false;

    void Start()
    {
        originalScale = transform.localScale;
        childObjectVisual = gameObject.transform.GetChild(0).gameObject;
        //originalPos = transform.position;

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        //canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
   
        currentEvent = stateOfEvent.hover;

        // Scale up the hovered card
        scaleTween = LeanTween.scale(childObjectVisual, originalScale * hoverScale, transitionTime);

        //// Move the card slightly up in world space
        float targetY = transform.position.y + hoverHeight;
        localMoveTween = LeanTween.moveY(gameObject, targetY, transitionTime);

        //save and then make the angle 0
        saveRotation = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

        gameObject.GetComponent<Canvas>().sortingOrder = 999;

        HandManager.Instance.PushNeightbourCards(this.gameObject);
        // StartCoroutine(WaitForOtherAnimationsToBeDone(HandManager.Instance.resetSpeed));




    }


    public void OnPointerExit(PointerEventData eventData)
    {
  
        currentEvent = stateOfEvent.exithover;
        index = -1; // Reset the index when the pointer exits the card

        //reset the position of all other cards
        HandManager.Instance.PushNeightbourCards(null);

        // Scale down the card
        scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

        //// Move the card back
        localMoveTween = LeanTween.moveY(gameObject, originalPosY, transitionTime);

        //rotate back the card on how it was
        gameObject.transform.eulerAngles = saveRotation;

        gameObject.GetComponent<Canvas>().sortingOrder = sortOrder;

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        //canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
            rectTransform.position = canvas.transform.TransformPoint(localPoint);

            //check activation
            canActivate = HandManager.Instance.CheckActivation(rectTransform);

            if (canActivate && gameObject.GetComponent<CardScript>().scriptableCard.primaryManaCost <= CombatManager.Instance.manaAvailable)
            {
                //activation should not be visible
                //green color
                gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = new Color32(35, 207 , 40, 100);
            }
            else
            {
                //red color
                gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = new Color32(255, 0, 0, 100);
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        //canvasGroup.blocksRaycasts = true;

        //no color 
        gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = new Color32(0, 0, 0, 0);

        if (canActivate && gameObject.GetComponent<CardScript>().scriptableCard.primaryManaCost <= CombatManager.Instance.manaAvailable)
        {
            //do the effects 
            DeckManager.Instance.PlayCard(gameObject.GetComponent<CardScript>());
        }

        HandManager.Instance.SetHandCards();
    }



}
