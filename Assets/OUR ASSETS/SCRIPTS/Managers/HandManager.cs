using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance;

    public List<GameObject> cardsInHandList; // Array to hold card GameObjects
    public List<GameObject> points;

    public int turnHandCardsLimit = 5;
    public int maxHandCardsLimit = 10;

    //
    public float angleDelta = 0;
    public float[] angleDeltaArray;
    // public float radius = 0;
    //public float radiusMultiplier = 0.1f; // Multiplier to control the radius
    public float radiusValue = 25f;
    public GameObject centerObject;

    public float pushingSpeed = 0.2f;
    public float pushingDistance = 200f;
    public float pushingLimit = 200f;
    public float pushingMultiplierLeft = 0.1f;
    public float pushingMultiplierRight = 0.1f;
    //public float[] pushingDistanceArray;
    //public float[] pushingDistanceFavorRight;
    public float resetSpeed = 0.2f;
    public float drawSpeed = 1f;

    public float adjustObjectYAxis = 0f;

    //ACTIVATION AREA
    [SerializeField] private RectTransform activateArea;
    [SerializeField] private float activationThreshold = 0.5f;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LeanTween.init(4000);

    }

    public void Start()
    {
        //SetHandCards();



    }






    public void SetHandCards()
    {
        foreach (GameObject cardRef in cardsInHandList)
        {
            DeckManager.Instance.UpdateCardDescription(cardRef);
            SetCardPosition(cardRef);
        }
    }

    public void SetHandCard(GameObject cardRef)
    {
        SetCardPosition(cardRef);
    }

    public void PushNeightbourCards(GameObject cardRef)
    {
        int index;
        int cardPos = 0;

        if (cardRef != null)
        {
            index =cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardID == cardRef.GetComponent<CardScript>().cardID);
        }
        else
        {
            index = -1;
        }



        foreach (GameObject card in cardsInHandList)
        {

            if (index != -1)
            {

                //get how far is the card from index 5 3
                int howManyCardsFromIndex = Mathf.Abs(cardPos - index);

                //float screenHeight = Screen.height;
                float limitPushBasedOnHand = ((maxHandCardsLimit + 1) - cardsInHandList.Count);

                //float pushLeft = (screenHeight / limitPushBasedOnHand) * pushingMultiplierLeft;
                //float pushRight = (screenHeight / limitPushBasedOnHand) * pushingMultiplierRight;

                float tempPushingDistance = pushingDistance;
                float pushLeft = 0;
                float pushRight = 0;

                if (cardsInHandList.Count < 6)
                {
                    pushLeft = ((pushingDistance / 2) * cardsInHandList.Count) * pushingMultiplierLeft;
                    pushRight = ((pushingDistance / 2)  * cardsInHandList.Count) * pushingMultiplierRight;
                }
                else
                {
                    pushLeft = (pushingDistance * cardsInHandList.Count) * pushingMultiplierLeft;
                    pushRight = (pushingDistance * cardsInHandList.Count) * pushingMultiplierRight;
                }

                //push more the more cards you have in the hand

                if (cardPos < index)
                {

                    //save the original position
                    //card.GetComponent<CardEvents>().originalPos = card.transform;

                    //push left based on the distance wanted
                    float posX = card.GetComponent<CardEvents>().originalPosX + (-1 * pushLeft); //+ (20 * Mathf.Abs(cardPos - index));
                    card.GetComponent<CardEvents>().moveTween = LeanTween.moveX(card, posX, pushingSpeed);
                }
                else if (cardPos > index)
                {
                    //save the original position
                    //card.GetComponent<CardEvents>().originalPos = card.transform;

                    ////push right
                    float posX = card.GetComponent<CardEvents>().originalPosX + pushRight;// - (20 * ( cardPos - index));
                    card.GetComponent<CardEvents>().moveTween = LeanTween.moveX(card, posX, pushingSpeed);
                }
                else
                {
                    //do nothing (its the index card)
                }

            }
            else
            {
                float posX = card.GetComponent<CardEvents>().originalPosX;
                card.GetComponent<CardEvents>().moveTween = LeanTween.moveX(card, posX, pushingSpeed);
            }
            //increase the card position
            cardPos++;

        }

    }

    public void AdjustCenterObject()
    {
        // Get the RectTransform of the centerObject
        RectTransform rectTransform = centerObject.GetComponent<RectTransform>();

        // Set its anchored position to the bottom center
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);
        rectTransform.pivot = new Vector2(0.5f, 0);

        // Adjust the Y position based on screen height
        rectTransform.anchoredPosition = new Vector2(0, adjustObjectYAxis);
    }

    public void SetCardPosition(GameObject cardRef)
    {

        AdjustCenterObject();


        int index = cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardID == cardRef.GetComponent<CardScript>().cardID); //FindCardIndex(cardRef.GetComponent<CardScript>());
  
        //The angle is based on how far the card is from the midpoint of the hand.
        //Note that the midpoint will either be a whole number or x.5
        float midpoint = (cardsInHandList.Count - 1) / 2f;

        float angleCalc =angleDeltaArray[cardsInHandList.Count-1]  ;//(angleDelta / cardsInHandList.Count * 2) + 1.2f;

        float angle = angleCalc * (midpoint - index);

        //Positive angles rotate counterclockwise, negative angles rotate clockwise
        cardsInHandList[index].transform.eulerAngles = new Vector3(0, 0, angle);

        //Mathf uses radians
        //A card that is rotated counterclockwise is on the left side of the hand,
        //while a card rotated clockwise should be on the right side of the hand.
        //This means we need to flip either the angle or the x value when calculating the
        //position.
        angle *= -Mathf.Deg2Rad;

        // Adjust the radius based on the screen height
        //float screenHeight = Screen.height;
        //float radius = screenHeight * radiusMultiplier;
        //float radius = 25f; 

        float x = Mathf.Sin(angle) * radiusValue;
        float y = Mathf.Cos(angle) * radiusValue;
        //cards[index].transform.position = new Vector3(centerObject.transform.position.x + x, centerObject.transform.position.y + y, 0);

        // Calculate the target position in world space
        Vector3 targetPosition = new Vector3(centerObject.transform.position.x + x, (centerObject.transform.position.y + y) - radiusValue, 0);

        // Convert target position from world space to canvas space
        Vector3 canvasPosition = SystemManager.Instance.uiCamera.WorldToScreenPoint(targetPosition);
        canvasPosition.z = 0; // Set z to 0 because we are working in 2D space

        // Set the position relative to the bottom center of the screen
        //Vector3 targetPosition = new Vector3(centerObject.transform.position.x + x, (centerObject.transform.position.y + y) - radius, 0);
        LeanTween.move(cardsInHandList[index], targetPosition, drawSpeed);
        //LeanTween.move(cardsInHandList[index], new Vector3(centerObject.transform.position.x + x, centerObject.transform.position.y + y, 0), drawSpeed);

        //save the original position
        cardsInHandList[index].GetComponent<CardEvents>().originalPosX = centerObject.transform.position.x + x;
        cardsInHandList[index].GetComponent<CardEvents>().originalPosY = (centerObject.transform.position.y + y) - radiusValue;

        //save the sort order
        cardsInHandList[index].GetComponent<CardEvents>().sortOrder = index;
        cardsInHandList[index].GetComponent<Canvas>().sortingOrder = index;
    }

    //public int FindCardIndex(CardScript cardScript)
    //{

    //    // Iterate through the list of cards to find the index of the specified card
    //    for (int i = 0; i < cardsInHandList.Count; i++)
    //    {
    //        if (cardsInHandList[i].GetComponent<CardScript>().cardID == cardScript.cardID)
    //        {
    //            return i; // Return the index if the card is found
    //        }
    //    }
    //    return -1; // Return -1 if the card is not found
    //}


    //public bool CheckActivation(RectTransform rectTransform)
    //{
    //    bool insideArea = false;

    //    if (activateArea == null) return false;

    //    Vector2 activateAreaScreenPos = RectTransformUtility.WorldToScreenPoint(null, activateArea.position);
    //    Vector2 cardScreenPos = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);

    //    float screenHeight = Screen.height;

    //    float activateAreaY = screenHeight * (1 - activationThreshold);

    //    if (cardScreenPos.y >= activateAreaY)
    //    {
    //        insideArea = true;
    //    }

    //    return insideArea;
    //}

    public bool CheckActivation(RectTransform rectTransform)
    {
        if (activateArea == null || SystemManager.Instance.uiCamera == null) return false;

        // Get the screen position of the activateArea and the rectTransform using the canvas camera
        Vector2 activateAreaScreenPos = RectTransformUtility.WorldToScreenPoint(SystemManager.Instance.uiCamera, activateArea.position);
        Vector2 cardScreenPos = RectTransformUtility.WorldToScreenPoint(SystemManager.Instance.uiCamera, rectTransform.position);

        // Calculate the activation threshold in screen coordinates
        float screenHeight = Screen.height;
        float activateAreaY = screenHeight * (1 - activationThreshold);

        // Check if the card is within the activation area
        return cardScreenPos.y >= activateAreaY;
    }

}
