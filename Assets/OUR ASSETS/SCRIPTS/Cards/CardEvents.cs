using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private float hoverScale = 1.2f;
    private Vector3 originaChoiceScale = new Vector3(1,1,1);
    private float hoverChoiceScale = 1.3f;
    private float transitionTime = 0.1f;
    private float hoverHeight = 0.6f;

    public int sortOrder = 0;

    private Vector3 originalScale;

    public Vector3 saveRotation;


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
    public Canvas canvas;
    private CanvasGroup canvasGroup;


    private bool canActivate = false;
    private bool isDragging = false;

    public bool isPointerInside = false;



    public GraphicRaycaster graphicRaycaster;
    public PointerEventData pointerEventData;
    public EventSystem eventSystem;

    void Start()
    {
        originalScale = transform.localScale;
        childObjectVisual = gameObject.transform.GetChild(0).gameObject;
        //originalPos = transform.position;

        rectTransform = GetComponent<RectTransform>();
        canvas = this.transform.parent.parent.GetComponentInParent<Canvas>();

        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

    }

    void Update()
    {
        //try
        //{
        //    DeckManager.Instance.UpdateCardUI(this.gameObject);


        //}
        //catch(Exception ex)
        //{
        //    string error = ex.ToString();
        //}
    }


    #region EVENTS

    public void OnPointerEnter(PointerEventData eventData)
    {



       // if (isPointerInside) return; // Prevent duplicate calls

        isPointerInside = true;

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            OnPointerEnter_TargetMode();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.CHOICE) {
            OnPointerEnter_ChoiceMode();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.SHIELDCHOICE)
        {
            OnPointerEnter_ShieldChoiceMode();
        }

    }

    private void OnEnable()
    {
        // Wait one frame to ensure the UI updates first
        StartCoroutine(DelayedCheck());
    }

    private IEnumerator DelayedCheck()
    {
        yield return null; // Wait one frame
        if (IsPointerOverUIElement(gameObject))
        {
            OnPointerEnter(null); // Manually trigger OnPointerEnter
        }
    }

    // Checks if the mouse is over this UI element
    private bool IsPointerOverUIElement(GameObject obj)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject == obj)
            {
                isPointerInside = true;
                return true;
            }
      
        }

        return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {



        isPointerInside = false;

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            OnPointerExit_TargetMode();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.CHOICE)
        {
            OnPointerExit_ChoiceMode();
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.SHIELDCHOICE)
        {
            OnPointerExit_ShieldChoiceMode();
        }


    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            OnPointerDown_TargetMode(eventData);
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.CHOICE)
        {
            OnPointerDown_ChoiceMode(eventData);
        }
        else if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.SHIELDCHOICE)
        {
            OnPointerDown_ShieldChoiceMode(eventData);
        }

    }



    public void OnDrag(PointerEventData eventData)
    {

        if (!UI_Combat.Instance.uiCombatEnable)
        {
            return;
        }

        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            OnDrag_TargetMode(eventData);
        }



    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (!UI_Combat.Instance.uiCombatEnable)
        {
            return;
        }


        if (SystemManager.Instance.abilityMode == SystemManager.AbilityModes.NONE)
        {
            OnPointerUp_TargetMode(eventData);
        }


    }

    #endregion



    //TARGET MODE
    #region TARGETMODE
    public void OnPointerEnter_TargetMode()
    {
        currentEvent = stateOfEvent.hover;

        // Scale up the hovered card
        scaleTween = LeanTween.scale(childObjectVisual, originalScale * hoverScale, transitionTime);

        //// Move the card slightly up in world space
        //float targetY = transform.position.y + hoverHeight;
        float targetY = HandManager.Instance.centerObject.transform.position.y + hoverHeight;
        localMoveTween = LeanTween.moveY(gameObject, targetY, transitionTime);

        //save and then make the angle 0
        //saveRotation = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

        gameObject.GetComponent<Canvas>().sortingOrder = 999;

        HandManager.Instance.PushNeightbourCards(this.gameObject);
    }

    public void OnPointerExit_TargetMode()
    {

        if (isDragging)
        {
            isDragging = false;
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

    public void OnPointerDown_TargetMode(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {

            isDragging = false;
        }
        else
        {

            isDragging = true;
        }
    }

    public void OnDrag_TargetMode(PointerEventData eventData)
    {

        if (isDragging)
        {

            isPointerInside = true;


            Vector2 screenPoint = eventData.position;

            // Clamp the mouse position to stay within screen bounds
            screenPoint.x = Mathf.Clamp(screenPoint.x, 0, Screen.width);
            screenPoint.y = Mathf.Clamp(screenPoint.y, 0, Screen.height);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPoint);

            rectTransform.position = canvas.transform.TransformPoint(localPoint);

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
            //rectTransform.position = canvas.transform.TransformPoint(localPoint);

            //check activation
            canActivate = HandManager.Instance.CheckActivation(rectTransform);

            if (canActivate && gameObject.GetComponent<CardScript>().cardScriptData.primaryManaCost <= Combat.Instance.manaAvailable /*&& SystemManager.Instance.thereIsActivatedCard == false*/
                && gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard.cardType != SystemManager.CardType.Curse)
            {
                ScriptableCard scriptableCard = gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard;

                //check if there is more than 1 target
                List<string> tagList = new List<string>();

                foreach (SystemManager.EntityTag entityTag in scriptableCard.targetEntityTagList)
                {
                    tagList.Add(entityTag.ToString());
                }

                //depending on the tags
                List<GameObject> targetPosList = Combat.Instance.FindPosTargeting(tagList);

                List<GameObject> targets = SystemManager.Instance.FindGameObjectsWithTags(tagList);

                targets.AddRange(targetPosList);

                //activation should not be visible
                if (scriptableCard.targetEntityTagList.Count > 0 && targets.Count > 1)
                {
                    OnPointerUp(null);
                }

                //green color
                gameObject.transform.GetChild(0).Find("UtilityBack").Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorActivationSuccess);
            }
            else
            {

                //red color
                gameObject.transform.GetChild(0).Find("UtilityBack").Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorActivationFail);
            }

        }
    }

    public void OnPointerUp_TargetMode(PointerEventData eventData)
    {
        //no color 
        gameObject.transform.GetChild(0).Find("UtilityBack").Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);


        //if it cancel drag
        if (isDragging == false /*|| SystemManager.Instance.thereIsActivatedCard == true*/ || gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard.cardType == SystemManager.CardType.Curse)
        {
            //reset everything
            //remove line renderer
            UI_Combat.Instance.cardLineRenderer.gameObject.SetActive(false);

            //return everything where it was
            HandManager.Instance.SetHandCards();

            // Scale down the card
            scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

            //leave from target
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

            return;
        }

        isDragging = false;
        //canvasGroup.blocksRaycasts = true;



        ScriptableCard scriptableCard = gameObject.GetComponent<CardScript>().cardScriptData.scriptableCard;

        if (canActivate && gameObject.GetComponent<CardScript>().cardScriptData.primaryManaCost <= Combat.Instance.manaAvailable)
        {

            // Scale down the card
            scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

            //check if there is more than 1 target
            List<string> tag = new List<string>();
           
            foreach (SystemManager.EntityTag entityTag in scriptableCard.targetEntityTagList)
            {
                tag.Add(entityTag.ToString());
            }

            List<GameObject> targets = SystemManager.Instance.FindGameObjectsWithTags(tag);

            //if there are targetable entities
            if (scriptableCard.targetEntityTagList.Count > 0 && targets.Count > 1)
            {
                //if the card is targetable

                //enter click mode which will disable all events from the cards
                SystemManager.Instance.abilityMode = SystemManager.AbilityModes.TARGET;

                CombatCardHandler.Instance.targetUIElement = this.gameObject.GetComponent<RectTransform>();

                CombatCardHandler.Instance.targetClicked = null;

                //when mouse click on sprite do the shit
                HandManager.Instance.SetHandCard(this.gameObject);
            }
            else
            {
                //if the target is 1 then we add the target as clicked
                if (targets.Count != 0)
                {
                    CombatCardHandler.Instance.targetClicked = targets[0];
                }

                // Scale down the card
                scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);


                //put it on the list
                PlayedCard playedCard = new PlayedCard();
                playedCard.timer = scriptableCard.waitOnQueueTimer;
                playedCard.target = CombatCardHandler.Instance.targetClicked;
                playedCard.scriptableCard = scriptableCard;
                playedCard.cardScriptData = gameObject.GetComponent<CardScript>().cardScriptData;
                playedCard.cardScript = gameObject.GetComponent<CardScript>();
                playedCard.cardObject = this.gameObject;

                //decrease available mana
                Combat.Instance.ManaAvailable -= playedCard.cardScriptData.primaryManaCost;

                //add it on the list
                Combat.Instance.playedCardList.Add(playedCard);

                //save the cardScript temp
                CardScriptData tempCardScriptData = new CardScriptData();
                tempCardScriptData = playedCard.cardScriptData;
                //remove from hand and add it to the played card
                DeckManager.Instance.RemovePlayedCardFromHand(tempCardScriptData);

                playedCard.playedCardUI = UI_Combat.Instance.AddPlayedCardUI(playedCard);

                //do the effects 
                //DeckManager.Instance.PlayerPlayedCard(gameObject.GetComponent<CardScript>());

                //return everything where it was
                HandManager.Instance.SetHandCards();
            }
        }
        else
        {
            // Scale down the card
            scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);

            //return everything where it was
            HandManager.Instance.SetHandCards();
        }

    }

    #endregion

    //CHOICE MODE
    #region CHOICEMODE
    public void OnPointerEnter_ChoiceMode()
    {
        // Scale up the hovered card
        scaleTween = LeanTween.scale(childObjectVisual, originaChoiceScale * hoverChoiceScale, transitionTime);
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);

        gameObject.transform.Find("Panel").Find("UtilityBack").Find("ChooseActivation").gameObject.SetActive(true);
    }

    public void OnPointerExit_ChoiceMode()
    {
        // Scale down the card
        scaleTween = LeanTween.scale(childObjectVisual, originaChoiceScale, transitionTime);
        gameObject.transform.Find("Panel").Find("UtilityBack").Find("ChooseActivation").gameObject.SetActive(false);
    }

  
    public void OnPointerDown_ChoiceMode(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DeckManager.Instance.AddCardToList(this.gameObject.GetComponent<CardScript>().cardScriptData);
        }
    }



    #endregion

    //SHIELD CHOICE MODE
    #region SHIELDCHOICEMODE
    public void OnPointerEnter_ShieldChoiceMode()
    {
        // Scale up the hovered card
        scaleTween = LeanTween.scale(childObjectVisual, originalScale * hoverChoiceScale, transitionTime);
        gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void OnPointerExit_ShieldChoiceMode()
    {
        // Scale down the card
        scaleTween = LeanTween.scale(childObjectVisual, originalScale, transitionTime);
    }


    public void OnPointerDown_ShieldChoiceMode(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Right)
        {



        }
        else
        {
            //close the thing 
            UIManager.Instance.ChooseGroupUI.SetActive(false);

            //resume
            SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

            //add the chosen shield (this is not scaleable)
            Combat.Instance.AdjustTargetHealth(null, CombatCardHandler.Instance.targetClicked, this.gameObject.GetComponent<CardScript>().cardScriptData.tempValue , false, SystemManager.AdjustNumberModes.SHIELD);

            //destroy children of the choice parent
            SystemManager.Instance.DestroyAllChildren(UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject);

        }
    }



    #endregion

}
