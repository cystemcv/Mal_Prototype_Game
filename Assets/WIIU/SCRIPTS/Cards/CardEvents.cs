using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private float hoverScale = 1.2f;
    private float transitionTime = 0.1f;
    private float hoverHeight = 70f;

    public int sortOrder = 0;

    private Vector3 originalScale;

    private Vector3 saveRotation;


    //private int index = -1; // Initialize the index to -1

    private GameObject childObjectVisual;

    public LTDescr scaleTween;
    public LTDescr moveTween;
    public LTDescr localMoveTween;

    //new variables that actually work
    public float originalPosX;
    public float originalPosY;

    public enum stateOfEvent { hover, exithover, clicked }

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

        if (CombatManager.Instance.targetMode)
        {
            return;
        }

        currentEvent = stateOfEvent.hover;

        // Scale up the hovered card
       scaleTween = LeanTween.scale(childObjectVisual, originalScale * hoverScale, transitionTime);

        //// Move the card slightly up in world space
        //float targetY = transform.position.y + hoverHeight;
        float targetY = HandManager.Instance.centerObject.transform.position.y + hoverHeight;
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

        if (CombatManager.Instance.targetMode)
        {
            return;
        }

        currentEvent = stateOfEvent.exithover;
        //index = -1; // Reset the index when the pointer exits the card

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
        if (CombatManager.Instance.targetMode || CombatManager.Instance.currentTurn != CombatManager.combatTurn.playerTurn)
        {
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {

            isDragging = false;
        }
        else
        {

            isDragging = true;
        }

        //canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (CombatManager.Instance.targetMode)
        {
            return;
        }



        if (isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
            rectTransform.position = canvas.transform.TransformPoint(localPoint);

            //check activation
            canActivate = HandManager.Instance.CheckActivation(rectTransform);

            if (canActivate && gameObject.GetComponent<CardScript>().primaryManaCost <= CombatManager.Instance.manaAvailable)
            {
                ScriptableCard scriptableCard = gameObject.GetComponent<CardScript>().scriptableCard;
                //activation should not be visible
                if (scriptableCard.canTarget)
                {
                    OnPointerUp(null);
                }

                //green color
                gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorActivationSuccess);
            }
            else
            {
                //red color
                gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorActivationFail);
            }

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        //no color 
        gameObject.transform.GetChild(0).Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);

        if (CombatManager.Instance.targetMode )
        {
            return;
        }

        Debug.Log("isDragging : " + isDragging);

        //if it cancel drag
        if (isDragging == false)
        {
            //reset everything
            //remove line renderer
            CombatManager.Instance.lineRenderer.gameObject.SetActive(false);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            // Scale down the card
            scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

            //leave from target
            CombatManager.Instance.targetMode = false;

            return;
        }

        isDragging = false;
        //canvasGroup.blocksRaycasts = true;



        ScriptableCard scriptableCard = gameObject.GetComponent<CardScript>().scriptableCard;

        if (canActivate && gameObject.GetComponent<CardScript>().primaryManaCost <= CombatManager.Instance.manaAvailable)
        {

            // Scale down the card
            scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

            if (scriptableCard.canTarget)
            {
                //if the card is targetable

                //enter click mode which will disable all events from the cards
                CombatManager.Instance.targetMode = true;

                CombatManager.Instance.targetUIElement = this.gameObject.GetComponent<RectTransform>();

                CombatManager.Instance.targetClicked = null;

                //when mouse click on sprite do the shit
                HandManager.Instance.SetHandCard(this.gameObject);
            }
            else
            {

                //if the card is not targetable

                //do the effects 
                DeckManager.Instance.PlayCard(gameObject.GetComponent<CardScript>());
                //return everything where it was
                HandManager.Instance.SetHandCards();
            }
        }
        else
        {
            //return everything where it was
            HandManager.Instance.SetHandCards();
        }


    }



}
