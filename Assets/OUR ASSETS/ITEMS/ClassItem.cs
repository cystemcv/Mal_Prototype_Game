using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Michsky.MUIP;

public class ClassItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public ScriptableItem scriptableItem;
    public int quantity = 0;
    public string itemID = "";
    public SystemManager.ItemIn itemIn = SystemManager.ItemIn.INVENTORY;

    public bool stopEvents = false;

    public int level = 1;

    public int tempValue = 0;

    public string customToolTip = "";

    public ClassItem(ScriptableItem scriptableItem, int quantity)
    {
        //add an id to this scriptableCard, this is in order to identify it by comparisons
        itemID = System.Guid.NewGuid().ToString();

        this.scriptableItem = scriptableItem;
        this.quantity = quantity;
    }

    // Method to set the data
    public void SetData(ClassItem item)
    {
        scriptableItem = item.scriptableItem;
        quantity = item.quantity;

        if (itemID == "")
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {

        if (stopEvents)
        {
            return;
        }

        if (customToolTip != "" && customToolTip != null)
        {
            this.gameObject.GetComponent<TooltipContent>().description = customToolTip;
        }
        else if (scriptableItem.itemDescription != "")
        {
            this.gameObject.GetComponent<TooltipContent>().description = scriptableItem.itemDescription;
        }
        else
        {
            this.gameObject.GetComponent<TooltipContent>().description = "";
        }

        if (itemIn == SystemManager.ItemIn.INVENTORY)
        {
            //change item border color
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorBlue);

            GameObject mainItemText = this.gameObject.transform.parent.parent.Find("ItemText").gameObject;

            //change inventory text
            mainItemText.GetComponent<TMP_Text>().text = "x" + quantity + " : " + scriptableItem.itemName;
        }
        else if (itemIn == SystemManager.ItemIn.LOOT)
        {
            //change item border color
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorBlue);

            GameObject mainItemText = this.gameObject.transform.parent.parent.Find("ItemText").gameObject;

            //change inventory text
            mainItemText.GetComponent<TMP_Text>().text = "x" + quantity + " : " + scriptableItem.itemName;
        }





    }

    public void OnPointerExit(PointerEventData eventData)
    {

        if (stopEvents)
        {
            return;
        }


        if (itemIn == SystemManager.ItemIn.INVENTORY)
        {
            //change item border color
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

            //change inventory text
            GameObject mainItemText = this.gameObject.transform.parent.parent.Find("ItemText").gameObject;

            //change inventory text
            mainItemText.GetComponent<TMP_Text>().text = "";
        }
        else if (itemIn == SystemManager.ItemIn.LOOT)
        {
            //change item border color
            this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

            //change inventory text
            GameObject mainItemText = this.gameObject.transform.parent.parent.Find("ItemText").gameObject;

            //change inventory text
            mainItemText.GetComponent<TMP_Text>().text = "";

   

    

        }


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemIn == SystemManager.ItemIn.INVENTORY)
        {

        }
        else if (itemIn == SystemManager.ItemIn.LOOT)
        {

            if (scriptableItem.itemCategory == SystemManager.ItemCategory.CARD)
            {
                ChooseCardToDeck();
            }
            else if (scriptableItem.itemCategory == SystemManager.ItemCategory.COMPANIONITEM)
            {
                AddCompanionItem();
            }
            else
            {
                //Remove from loot
                ItemManager.Instance.RemoveItemToParent(this.gameObject, UIManager.Instance.lootGO);

                //add to inventory
                ItemManager.Instance.AddItemToParent(this, UIManager.Instance.inventoryGO, SystemManager.ItemIn.INVENTORY);
            }

            UIManager.Instance.lootGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";
            UIManager.Instance.inventoryGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";

        }

    }

    public void AddCompanionItem()
    {

        //add to companion list
        ItemManager.Instance.AddCompanionItemInList(this);

        if (quantity > 1)
        {
            quantity--;
            ItemManager.Instance.UpdateItemUI(this.gameObject, this);
        }
        else
        {
            ItemManager.Instance.RemoveItemToParent(this.gameObject, UIManager.Instance.lootGO);
        }


    }

    public void ChooseCardToDeck()
    {

        //get the pool
        List<CardListManager.CardPoolList> filteredCardListPool = CardListManager.Instance.cardPoolLists.Where(pool =>
        pool.mainClass == StaticData.staticCharacter.mainClass
        ).ToList();

        if (filteredCardListPool.Count <= 0)
        {
            return;
        }


        if (quantity > 1)
        {
            quantity--;
            ItemManager.Instance.UpdateItemUI(this.gameObject,this);
        }
        else
        {
            ItemManager.Instance.RemoveItemToParent(this.gameObject, UIManager.Instance.lootGO);
        }


        List<ScriptableCard> filteredCardList = filteredCardListPool[0].scriptableCards;


        //display screen
        UIManager.Instance.chooseACardScreen.SetActive(true);

        //change the mode
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.CHOICE;

        //where to add card
        SystemManager.Instance.addCardTo = SystemManager.AddCardTo.mainDeck;

        int cardsToChoose = 3;

        //get the cards to display
        for (int i = 0; i < cardsToChoose; i++)
        {
            //get the random card from the filtered list
            int randomIndex = UnityEngine.Random.Range(0, filteredCardList.Count);

            //go for next item
            if (randomIndex == -1)
            {
                continue;
            }

            //generate cardScript
            CardScript cardScriptTemp = new CardScript();
            cardScriptTemp.scriptableCard = filteredCardList[randomIndex];


            //generate the card and parent it
            DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UIManager.Instance.chooseACardScreen.transform.Find("CardContainer").gameObject, false, true);
        }

    }

}
