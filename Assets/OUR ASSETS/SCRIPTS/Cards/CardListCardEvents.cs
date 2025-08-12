using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardListCardEvents : MonoBehaviour, IPointerDownHandler
{

    public GameObject markedGO;
    public GameObject markedNumberGO;
    public CardScriptData cardScriptData;
    public ScriptableCard scriptableCard;


    // Start is called before the first frame update
    void Start()
    {
        if (CardListManager.Instance.CardExistsInList(UIManager.Instance.selectedCardList, cardScriptData))
        {
            CardScriptData cardScriptDataFound = CardListManager.Instance.FindCardByName(UIManager.Instance.selectedCardList, cardScriptData);
            cardScriptData = cardScriptDataFound;

            if (!UIManager.Instance.cardScriptAllowDuplicates)
            {
                markedGO.SetActive(true);
            }
            else
            {
                markedNumberGO.SetActive(true);
                markedNumberGO.transform.Find("Text").GetComponent<TMP_Text>().text = cardScriptData.copiesOfCard.ToString();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (UIManager.Instance.enableMaxSelection <= 0)
        {
            return;
        }

        if ((eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right) && !UIManager.Instance.cardScriptAllowDuplicates)
        {

            if (!markedGO.activeSelf && UIManager.Instance.selectedCardList.Count < UIManager.Instance.enableMaxSelection)
            {
                markedGO.SetActive(true);
                cardScriptData.copiesOfCard = 1;
                UIManager.Instance.selectedCardList.Add(cardScriptData);
            }
            else if (markedGO.activeSelf)
            {
                markedGO.SetActive(false);
                cardScriptData.copiesOfCard = 0;
                DeckManager.Instance.RemoveCardFromList(cardScriptData, UIManager.Instance.selectedCardList);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left && UIManager.Instance.cardScriptAllowDuplicates)
        {

            if (UIManager.Instance.selectedCardList.Count < UIManager.Instance.enableMaxSelection)
            {
                //check 
                if (CardListManager.Instance.CardExistsInList(UIManager.Instance.selectedCardList, cardScriptData))
                {
                    CardScriptData cardScriptDataFound = CardListManager.Instance.FindCardByName(UIManager.Instance.selectedCardList, cardScriptData);
                    cardScriptDataFound.copiesOfCard += 1;
                    cardScriptData.copiesOfCard = cardScriptDataFound.copiesOfCard;
                }
                else
                {
                    cardScriptData.copiesOfCard += 1;
                    UIManager.Instance.selectedCardList.Add(cardScriptData);
                }

                markedNumberGO.SetActive(true);
                markedNumberGO.transform.Find("Text").GetComponent<TMP_Text>().text = cardScriptData.copiesOfCard.ToString();

            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right && UIManager.Instance.cardScriptAllowDuplicates && cardScriptData.copiesOfCard > 0)
        {
            Debug.Log("Right click detected");

            if (CardListManager.Instance.CardExistsInList(UIManager.Instance.selectedCardList, cardScriptData))
            {
                CardScriptData cardScriptDataFound = CardListManager.Instance.FindCardByName(UIManager.Instance.selectedCardList, cardScriptData);
                cardScriptDataFound.copiesOfCard -= 1;
                cardScriptData.copiesOfCard = cardScriptDataFound.copiesOfCard;

                if (cardScriptData.copiesOfCard == 0)
                {
                    markedNumberGO.SetActive(false);
                    markedNumberGO.transform.Find("Text").GetComponent<TMP_Text>().text = cardScriptData.copiesOfCard.ToString();
                }
                else
                {
                    markedNumberGO.SetActive(true);
                    markedNumberGO.transform.Find("Text").GetComponent<TMP_Text>().text = cardScriptData.copiesOfCard.ToString();
                }
            }
        }
        


        GameObject selectionText = UIManager.Instance.cardListGO.transform.Find("Others").Find("SelectionText").gameObject;
        int totalCopies = UIManager.Instance.selectedCardList.Sum(card => card.copiesOfCard);
        selectionText.GetComponent<TMP_Text>().text = totalCopies + "/" + UIManager.Instance.enableMaxSelection;

    }
}
