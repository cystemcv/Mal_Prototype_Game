using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Michsky.MUIP;
using System.Linq;

public class ClassItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public ScriptableItem scriptableItem;
    public int quantity = 0;
    public string itemID = "";
    public SystemManager.ItemIn itemIn = SystemManager.ItemIn.INVENTORY;

    public bool stopEvents = false;

    public int level = 0;

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
        level = item.level;
        customToolTip = item.customToolTip;

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

        //change item border color
        //this.gameObject.GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorBlue);

        if (itemIn == SystemManager.ItemIn.INVENTORY)
        {
            //change inventory text
            UIManager.Instance.inventoryText.text = "x" + quantity + " : " + scriptableItem.itemName;
        }
        else if (itemIn == SystemManager.ItemIn.ARTIFACTS)
        {
            //change inventory text
            UIManager.Instance.artifactText.text = "x" + quantity + " : " + scriptableItem.itemName;
        }
        else if (itemIn == SystemManager.ItemIn.COMPANION)
        {
            //change inventory text
            UIManager.Instance.companionText.text = "x" + quantity + " : " + scriptableItem.itemName;
        }
        else if (itemIn == SystemManager.ItemIn.LOOT)
        {
            //change inventory text
            UIManager.Instance.lootText.text = "x" + quantity + " : " + scriptableItem.itemName;
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
            //change inventory text
            UIManager.Instance.inventoryText.text = "";
        }
        else if (itemIn == SystemManager.ItemIn.ARTIFACTS)
        {
            //change inventory text
            UIManager.Instance.artifactText.text = "";
        }
        else if (itemIn == SystemManager.ItemIn.COMPANION)
        {
            //change inventory text
            UIManager.Instance.companionText.text = "";
        }
        else if (itemIn == SystemManager.ItemIn.LOOT)
        {
            //change inventory text
            UIManager.Instance.lootText.text = "";
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
            else if (scriptableItem.itemCategory == SystemManager.ItemCategory.RANDOMCOMPANIONITEM)
            {
                AddRandomCompanionItem();
            }
            else if (scriptableItem.itemCategory == SystemManager.ItemCategory.RANDOMARTIFACTITEM)
            {
                AddRandomArtifactItem();
            }
            else
            {
                //add to inventory
                ItemManager.Instance.AddInventoryItemInList(this);

                //Remove from loot
                ItemManager.Instance.RemoveItemFromListGOFromLoot(this, StaticData.lootItemList);

                ItemManager.Instance.ShowLoot();
                ItemManager.Instance.RefreshInventory();
            }

            ItemManager.Instance.ClearAllText();
        }

    }


    

    public void AddCompanionItem()
    {

        //add to companion list
        ItemManager.Instance.AddCompanionItemInList(this);

        //Remove from loot
        ItemManager.Instance.RemoveItemFromListGOFromLoot(this, StaticData.lootItemList);

        //ItemManager.Instance.ShowInventory();
        ItemManager.Instance.ShowLoot();
  


    }


    public void AddRandomArtifactItem()
    {


        //Remove from loot
        ItemManager.Instance.RemoveItemFromListGOFromLoot(this, StaticData.lootItemList);

        //ItemManager.Instance.ShowInventory();
        ItemManager.Instance.ShowLoot();
    

        int itemsToChoose = 3;

        // Filter the artifact pool to exclude items already in StaticData.artifactItemList
        List<ScriptableItem> filteredArtifactPool;

        if (ItemManager.Instance.artifactTestPoolList.Count != 0)
        {
           filteredArtifactPool = ItemManager.Instance.artifactTestPoolList
    .Where(scriptableItem => !StaticData.artifactItemList
        .Any(classItem => classItem.scriptableItem == scriptableItem))
    .ToList();
        }
        else
        {
             filteredArtifactPool = ItemManager.Instance.artifactPoolList
    .Where(scriptableItem => !StaticData.artifactItemList
        .Any(classItem => classItem.scriptableItem == scriptableItem))
    .ToList();
        }


        // Check if there are any items left after filtering
        if (filteredArtifactPool.Count == 0)
        {
            Debug.LogWarning("No artifact on the pool list");
            return;
        }

        GameObject parent = UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject;

        //destroy previous choices
        SystemManager.Instance.DestroyAllChildren(parent);

        // Select random items from the filtered list
        for (int i = 0; i < itemsToChoose; i++)
        {
            // Generate a random index from the filtered list
            int randomIndex = UnityEngine.Random.Range(0, filteredArtifactPool.Count);

            // Create a ClassItem for the randomly selected item
            ClassItem itemClassTemp = new ClassItem(filteredArtifactPool[randomIndex], 1);

            // Instantiate the item prefab
            GameObject itemPrefab = Instantiate(
                ItemManager.Instance.itemChoosePrefab,
                parent.transform.position,
                Quaternion.identity);

            //set it as a child of the parent
            itemPrefab.transform.SetParent(parent.transform);

            itemPrefab.transform.localScale = Vector3.one;


            //ui
            itemPrefab.transform.Find("Icon").GetComponent<Image>().sprite = itemClassTemp.scriptableItem.Icon;
            itemPrefab.transform.Find("Title").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemName;
            itemPrefab.transform.Find("Description").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemDescription;
            itemPrefab.transform.Find("ItemLevel").gameObject.SetActive(false);

            // Assign the item to the ItemChoiceClass component
            ItemChoiceClass itemChoiceClass = itemPrefab.AddComponent<ItemChoiceClass>();
            itemChoiceClass.classItem = itemClassTemp;
    
        }

        UIManager.Instance.ChooseGroupUI.SetActive(true);
        UIManager.Instance.ChooseGroupUI.transform.Find("TITLE").GetComponent<TMP_Text>().text = "CHOOSE AN ARTIFACT!";


    }

    public void AddRandomCompanionItem()
    {


        //Remove from loot
        ItemManager.Instance.RemoveItemFromListGOFromLoot(this, StaticData.lootItemList);

        //ItemManager.Instance.ShowInventory();
        ItemManager.Instance.ShowLoot();
     

        int itemsToChoose = 3;

        // Get the companion item list and filter out items at max level
        List<ScriptableItem> companionSOItems = StaticData.staticScriptableCompanion.companionItemList
            .Where(item => !ItemManager.Instance.CheckIfItemMaxLevel(new ClassItem(item, 1)))
            .ToList();

        // Check if there are any items left after filtering
        if (companionSOItems.Count == 0)
        {
            Debug.LogWarning("All companion items are at max level. No items to choose from.");
            return;
        }

        GameObject parent = UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject;

        //destroy previous choices
        SystemManager.Instance.DestroyAllChildren(parent);

        // Select random items from the filtered list
        for (int i = 0; i < itemsToChoose; i++)
        {
            // Generate a random index from the filtered list
            int randomIndex = UnityEngine.Random.Range(0, companionSOItems.Count);

            // Create a ClassItem for the randomly selected item
            ClassItem itemClassTemp = new ClassItem(companionSOItems[randomIndex], 1);

            // Instantiate the item prefab
            GameObject itemPrefab = Instantiate(
                ItemManager.Instance.itemChoosePrefab,
                parent.transform.position,
                Quaternion.identity);

            //set it as a child of the parent
            itemPrefab.transform.SetParent(parent.transform);

            itemPrefab.transform.localScale = Vector3.one;


            //ui
            itemPrefab.transform.Find("Icon").GetComponent<Image>().sprite = itemClassTemp.scriptableItem.Icon;
            itemPrefab.transform.Find("Title").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemName;
            itemPrefab.transform.Find("Description").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemDescription;
            itemPrefab.transform.Find("ItemLevel").GetComponent<TMP_Text>().text = "LV" + ItemManager.Instance.GetItemLevelFromList(itemClassTemp);

            // Assign the item to the ItemChoiceClass component
            ItemChoiceClass itemChoiceClass = itemPrefab.AddComponent<ItemChoiceClass>();
            itemChoiceClass.classItem = itemClassTemp;
        }

        UIManager.Instance.ChooseGroupUI.SetActive(true);
        UIManager.Instance.ChooseGroupUI.transform.Find("TITLE").GetComponent<TMP_Text>().text = "CHOOSE COMPANION UPGRADE!";


    }


    public void ChooseCardToDeck()
    {

        GameObject parent = UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject;

        //destroy previous choices
        SystemManager.Instance.DestroyAllChildren(parent);

        //get the pool
        List<CardListManager.CardPoolList> filteredCardListPool = CardListManager.Instance.cardPoolLists.Where(pool =>
        pool.mainClass == StaticData.staticCharacter.mainClass
        ).ToList();

        if (filteredCardListPool.Count <= 0)
        {
            return;
        }


        //Remove from loot
        ItemManager.Instance.RemoveItemFromListGOFromLoot(this, StaticData.lootItemList);

        //ItemManager.Instance.ShowInventory();
        ItemManager.Instance.ShowLoot();
  
        List<ScriptableCard> filteredCardList = filteredCardListPool[0].scriptableCards;


        //display screen
        UIManager.Instance.ChooseGroupUI.SetActive(true);
        UIManager.Instance.ChooseGroupUI.transform.Find("TITLE").GetComponent<TMP_Text>().text = "CHOOSE A CARD!";

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
            DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject, false, true);
        }

    }

}
