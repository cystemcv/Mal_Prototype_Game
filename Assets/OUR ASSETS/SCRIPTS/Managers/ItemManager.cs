using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using System.Linq;
using DG.Tweening;

public class ItemManager : MonoBehaviour
{

    public List<ScriptableItem> scriptableItemsCheats = new List<ScriptableItem>();

    public static ItemManager Instance;

    public List<ScriptableItem> scriptableItemList;


    public List<ScriptableItem> artifactPoolList;

    public List<ScriptableItem> artifactTestPoolList;


    public GameObject itemChoosePrefab;

    private void Awake()
    {

        if (Instance == null)
        {
            //DOTween.Init();
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Start()
    {

        foreach (ScriptableItem scriptableItem in scriptableItemsCheats)
        {

            ClassItemData classItem = new ClassItemData(scriptableItem,9990);
            StaticData.inventoryItemList.Add(classItem);

        }

        //foreach (ScriptableItem scriptableItem in scriptableItemList)
        //{

        //    ClassItem classItem = new ClassItem(scriptableItem, Random.Range(0, 100));
        //    classItem.scriptableItem = scriptableItem;
        //    classItem.quantity = Random.Range(0, 20);

        //    //AddItemToParent(classItem, UIManager.Instance.inventoryGO, SystemManager.ItemIn.INVENTORY);
        //    AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);
        //    //StaticData.inventory.Add(classItem);

        //}

        //UIManager.Instance.lootGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";
        //UIManager.Instance.inventoryGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";
    }


    public string GenerateNewID()
    {
        return System.Guid.NewGuid().ToString();
    }

    //public void AddItemToParent(ClassItem classItem, GameObject itemParent, SystemManager.ItemIn itemIn)
    //{

    //    bool founditem = false;

    //    //on loot we dont want items to combine
    //    if (itemIn != SystemManager.ItemIn.LOOT) {

    //        //if the item is found then increase its quantity
    //        foreach (Transform child in itemParent.transform)
    //        {
    //            ClassItem uiClassItem = child.GetComponent<ClassItem>();

    //            if (uiClassItem != null && uiClassItem.scriptableItem.itemName == classItem.scriptableItem.itemName)
    //            {

    //                // If the item exists, increase its quantity
    //                uiClassItem.quantity += classItem.quantity;

    //                if (uiClassItem.quantity >= 999)
    //                {
    //                    uiClassItem.quantity = 999;
    //                }


    //                UpdateItemUI(child.gameObject, uiClassItem);
    //                founditem = true;
    //                break;
    //            }
    //        }
    //    }

    //    //if the item is not found then add it
    //    if (founditem == false)
    //    {
    //        // Find the first empty space
    //        foreach (Transform child in itemParent.transform)
    //        {
    //            ClassItem uiClassItem = child.GetComponent<ClassItem>();

    //            //find the first empty space
    //            if (uiClassItem == null)
    //            {
    //                // Update the UI component if needed, e.g., by updating a quantity text
    //                ClassItem classItemGO = new ClassItem(classItem.scriptableItem, classItem.quantity);


    //                //attach the script
    //                child.gameObject.AddComponent<ClassItem>();

    //                child.gameObject.GetComponent<ClassItem>().SetData(classItemGO);

    //                child.gameObject.GetComponent<ClassItem>().itemIn = itemIn;
    //                child.gameObject.GetComponent<ClassItem>().customToolTip = classItem.customToolTip;
    //                child.gameObject.GetComponent<ClassItem>().itemID = GenerateNewID();

    //                //update the ui
    //                UpdateItemUI(child.gameObject, classItemGO);
    //                break;
    //            }

    //        }
    //    }


    //}

    //public void UpdateItemUI(GameObject gameObject, ClassItem uiClassItem)
    //{

    //    //update the icon
    //    gameObject.transform.Find("Image").gameObject.SetActive(true);
    //    gameObject.transform.Find("Image").GetComponent<Image>().sprite = uiClassItem.scriptableItem.Icon;

    //    //updare the quantity
    //    gameObject.transform.Find("TextParent").gameObject.SetActive(true);
    //    gameObject.transform.Find("TextParent").Find("Text").GetComponent<TMP_Text>().text = uiClassItem.quantity.ToString();

    //    //tooltip
    //    //gameObject.GetComponent<TooltipContent>().description = uiClassItem.scriptableItem.itemDescription;
    //}

    //public void UpdateRemovedItemUI(GameObject gameObject)
    //{
    //    //update the icon
    //    gameObject.transform.Find("Image").gameObject.SetActive(false);
    //    gameObject.transform.Find("Image").GetComponent<Image>().sprite = null;

    //    //updare the quantity
    //    gameObject.transform.Find("TextParent").gameObject.SetActive(false);
    //    gameObject.transform.Find("TextParent").Find("Text").GetComponent<TMP_Text>().text = "";

    //    //tooltip
    //    //gameObject.GetComponent<TooltipContent>().description = "";
    //}

    public void RemoveItemFromListGOFromLoot(ClassItemData classItemToRemove, List<ClassItemData> classItemList)
    {
        // Find the index of the item with the same itemID
        int indexToRemove = classItemList.FindIndex(item => item.itemID == classItemToRemove.itemID);

        // If the item is found, remove it at the specified index
        if (indexToRemove != -1)
        {
            classItemList.RemoveAt(indexToRemove);
        }

    }

    public void ShowInventory()
    {
        UIManager.Instance.inventoryMain.SetActive(true);

        //activate animation
        //UIManager.Instance.inventoryMain.GetComponent<DoTweenAnimController>().PlayAnimation();

        //UIManager.Instance.lootMain.SetActive(false);

        ItemManager.Instance.ClearAllText();

        ResetGOObject(UIManager.Instance.inventoryGO);
        ResetGOObject(UIManager.Instance.companionGO);
        ResetGOObject(UIManager.Instance.artifactGO);

        PopulateGOObject(UIManager.Instance.inventoryGO, StaticData.inventoryItemList);
        PopulateGOObject(UIManager.Instance.companionGO, StaticData.companionItemList);
        PopulateGOObject(UIManager.Instance.artifactGO, StaticData.artifactItemList);
    }

    public void RefreshInventory()
    {
        ResetGOObject(UIManager.Instance.inventoryGO);
        PopulateGOObject(UIManager.Instance.inventoryGO, StaticData.inventoryItemList);
    }

    public void RefreshArtifacts()
    {
        ResetGOObject(UIManager.Instance.artifactGO);
        PopulateGOObject(UIManager.Instance.artifactGO, StaticData.artifactItemList);
    }

    public void RefreshCompanion()
    {
        ResetGOObject(UIManager.Instance.companionGO);
        PopulateGOObject(UIManager.Instance.companionGO, StaticData.companionItemList);
    }

    public void ClearAllText()
    {

        UIManager.Instance.inventoryText.text = "";
        UIManager.Instance.artifactText.text = "";
        UIManager.Instance.companionText.text = "";
        UIManager.Instance.inventoryTextDescription.text = "";
        UIManager.Instance.artifactTextDescription.text = "";
        UIManager.Instance.companionTextDescription.text = "";
    }

    public void ShowLoot()
    {
        UIManager.Instance.combatEndWindow.SetActive(true);
        UIManager.Instance.lootText.text = "";
        UIManager.Instance.lootTextDescription.text = "";

        ResetGOObject(UIManager.Instance.lootGO);
        PopulateGOObject(UIManager.Instance.lootGO, StaticData.lootItemList);
    }

    public void HideInventory()
    {
        //UIManager.Instance.inventoryMain.GetComponent<DoTweenAnimController>().PlayAnimationBackward();
        UIManager.Instance.inventoryMain.SetActive(false);
    }

    public void HideLoot()
    {
        UIManager.Instance.lootMain.SetActive(false);
    }



    public void HideLootParent()
    {
        HideItemParent(UIManager.Instance.lootGO);
    }




    public void ShowItemParent(GameObject itemParent)
    {
        itemParent.transform.parent.gameObject.SetActive(true);
    }

    public void HideItemParent(GameObject itemParent)
    {
        itemParent.transform.parent.gameObject.SetActive(false);
    }


    public IEnumerator ActivateItemList(SystemManager.ActivationType activationType, CardScript cardScript)
    {
        //use all lists of items
        yield return StartCoroutine(ActivateItems(activationType, StaticData.companionItemList, cardScript));
        yield return StartCoroutine(ActivateItems(activationType, StaticData.artifactItemList, cardScript));
    }

    public IEnumerator ActivateItems(SystemManager.ActivationType activationType, List<ClassItemData> classItems, CardScript cardScript)
    {
        foreach (var item in classItems)
        {
            if (item.scriptableItem.initializationType == activationType)
            {
                item.scriptableItem.Initialiaze(item, cardScript);
                yield return new WaitForSeconds(0.5f);
            }
        }

        foreach (var item in classItems)
        {
            if (item.scriptableItem.activationType == activationType)
            {
                item.scriptableItem.Activate(item, cardScript);
                yield return new WaitForSeconds(0.5f);
            }
        }

        foreach (var item in classItems)
        {
            if (item.scriptableItem.expiredType == activationType)
            {
                item.scriptableItem.Expired(item, cardScript);
                yield return new WaitForSeconds(0.5f);
            }
        }


    }

    public void GameStartItems()
    {
        foreach (var scriptableItem in ItemManager.Instance.artifactPoolList)
        {
            scriptableItem.GameStart();

        }

        foreach (var scriptableItem in StaticData.staticScriptableCompanion.companionItemList)
        {
            scriptableItem.GameStart();

        }
    }

    public ClassItemData SOItemToClass(ScriptableItem scriptableItem)
    {

        ClassItemData classItemTemp = new ClassItemData(scriptableItem, 0);
        return classItemTemp;

    }

    public void AddRemoveInventoryItemInList(ClassItemData classItem)
    {
        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.inventoryItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {

            ClassItemData itemFound = StaticData.inventoryItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            );

            // If the item exists, find it and increase its quantity based on the item quantity
            itemFound.quantity += classItem.quantity;

            //remove item
            if (itemFound.quantity <= 0)
            {
                ItemManager.Instance.RemoveItemFromListGOFromLoot(itemFound, StaticData.inventoryItemList);
            }
        }
        else
        {

            if (classItem.quantity < 0)
            {
                return;
            }

            //create new class
            ClassItemData classItemTemp = new ClassItemData(classItem.scriptableItem, classItem.quantity);
            classItemTemp.SetData(classItem);

            StaticData.inventoryItemList.Add(classItemTemp);
        }

    


    }

    public void AddCompanionItemInList(ClassItemData classItem)
    {
        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.companionItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {
            // If the item exists, find it and increase its level by 1
            StaticData.companionItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            ).level += 1;
        }
        else
        {
            // If the item doesn't exist, add it to the list
            classItem.level += 1; //from 0 to 1
            StaticData.companionItemList.Add(classItem);
        }
    }

    public void AddArtifactItemInList(ClassItemData classItem)
    {
        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.artifactItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (!itemExists)
        {
            StaticData.artifactItemList.Add(classItem);
        }
    }

    public bool CheckIfItemMaxLevel(ClassItemData classItem)
    {

        bool isMaxLevel = false;

        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.companionItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {
            // If the item exists, find it and increase its level by 1
            ClassItemData item = StaticData.companionItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            );

            //stop because its max level
            if (item.level == item.scriptableItem.maxLevel)
            {
                isMaxLevel = true;
            }
        }

        return isMaxLevel;
    }

    public int GetItemLevelFromList(ClassItemData classItem)
    {

        int itemLevel = 0;

        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.companionItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {
            // If the item exists, find it and increase its level by 1
            ClassItemData item = StaticData.companionItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            );

            itemLevel = item.level;
        }

        return itemLevel;
    }

    public void PopulateGOObject(GameObject goObject, List<ClassItemData> classItems)
    {

        // Get the companion item list


        // Loop through companion items and assign each to a child GameObject in goObject
        for (int i = 0; i < classItems.Count && i < goObject.transform.childCount; i++)
        {
            // Get the current item and child
            //classItems[i].gameObject.GetComponent<ClassItem>().enabled = true;
            ClassItemData classItem = classItems[i];
            Transform itemPrefab = goObject.transform.GetChild(i);

            // Activate the child GameObject and populate it with the item
            itemPrefab.gameObject.SetActive(true);

            foreach (Transform itemPrefabChild in itemPrefab)
            {
                itemPrefabChild.gameObject.SetActive(true);
            }

            // Assuming itemPrefab has a script (e.g., ItemDisplay) to show the item's details
            itemPrefab.GetComponent<ClassItem>().enabled = true;
            ClassItem itemDisplay = itemPrefab.GetComponent<ClassItem>();
            itemDisplay.classItemData = classItem;
            //itemDisplay.enabled = true;
            if (itemDisplay != null)
            {

                itemDisplay.classItemData.SetData(classItem);
                itemDisplay.classItemData.itemID = classItem.itemID;

                itemPrefab.GetChild(1).gameObject.GetComponent<Image>().sprite = itemDisplay.classItemData.scriptableItem.Icon;

                //level or quantity
                if (goObject == UIManager.Instance.inventoryGO)
                {
                    itemPrefab.GetChild(2).gameObject.GetComponent<TMP_Text>().text = itemDisplay.classItemData.quantity.ToString();
                    itemDisplay.classItemData.itemIn = SystemManager.ItemIn.INVENTORY;
                }
                else if (goObject == UIManager.Instance.lootGO)
                {
                    itemPrefab.GetChild(2).gameObject.GetComponent<TMP_Text>().text = itemDisplay.classItemData.quantity.ToString();
                    itemDisplay.classItemData.itemIn = SystemManager.ItemIn.LOOT;
                }
                else if (goObject == UIManager.Instance.artifactGO)
                {
                    itemPrefab.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";
                    itemDisplay.classItemData.itemIn = SystemManager.ItemIn.ARTIFACTS;
                }
                else
                {
                    itemPrefab.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "LV" + itemDisplay.classItemData.level.ToString();
                    itemDisplay.classItemData.itemIn = SystemManager.ItemIn.COMPANION;
                }


                //itemDisplay.SetItem(classItem);  // This is a custom method you would create to display item details
            }
        }

    }

    public void ResetGOObject(GameObject goObject)
    {
        //reset them back to blank state
        foreach (Transform itemPrefab in goObject.transform)
        {
            //itemPrefab.gameObject.SetActive(false);

            itemPrefab.GetComponent<ClassItem>().enabled = false;

            foreach (Transform itemPrefabChild in itemPrefab)
            {


                if (itemPrefabChild.name == "Background" || itemPrefabChild.name == "Outline")
                {
                    itemPrefabChild.gameObject.SetActive(true);
                }
                else
                {
                    itemPrefabChild.gameObject.SetActive(false);
                }

            }
        }

    }

    //public void OpenCompanionGO()
    //{

    //    //populate companionGO
    //    ResetGOObject(UIManager.Instance.companionGO);
    //    PopulateGOObject(UIManager.Instance.companionGO, StaticData.companionItemList);

    //    UIManager.Instance.companionGO.SetActive(true);
    //    UIManager.Instance.inventoryGO.SetActive(false);
    //    UIManager.Instance.artifactGO.SetActive(false);
    //}

    //public void OpenInventoryGO()
    //{

    //    //populate companionGO
    //    ResetGOObject(UIManager.Instance.inventoryGO);

    //    PopulateGOObject(UIManager.Instance.inventoryGO, StaticData.inventoryItemList);

    //    UIManager.Instance.inventoryGO.SetActive(true);
    //    UIManager.Instance.companionGO.SetActive(false);
    //    UIManager.Instance.artifactGO.SetActive(false);

    //}

    //public void OpenArtifactGO()
    //{

    //    //populate companionGO
    //    ResetGOObject(UIManager.Instance.artifactGO);

    //    PopulateGOObject(UIManager.Instance.artifactGO, StaticData.artifactItemList);

    //    UIManager.Instance.artifactGO.SetActive(true);
    //    UIManager.Instance.companionGO.SetActive(false);
    //    UIManager.Instance.inventoryGO.SetActive(false);
    //}

    public void AddItemOnActivateOrder(ScriptableItem scriptableItem, string toolTip, bool failed)
    {
        GameObject parentObj = UIManager.Instance.itemActivateOrderPanel;

        // Safeguard: Ensure the parent is not null
        if (parentObj == null)
        {
            Debug.LogWarning("Parent object is null!");
            return;
        }

        // Destroy excess children first
        while (parentObj.transform.childCount > 22)
        {
            // Destroy the oldest child
            DestroyImmediate(parentObj.transform.GetChild(0).gameObject);
        }

        // Instantiate the new item
        GameObject newItem = Instantiate(UIManager.Instance.ItemActivateOrderPrefab, parentObj.transform);
        newItem.transform.localScale = Vector3.one;

        // Configure the item
        newItem.transform.GetChild(0).gameObject.SetActive(true);
        newItem.transform.GetChild(0).GetComponent<Image>().sprite = scriptableItem.Icon;

        // Add the tooltip
        newItem.GetComponent<TooltipContent>().description = toolTip;

        // Set color based on success or failure
        Color itemColor = failed
            ? SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed)
            : SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorGreen);
        newItem.GetComponent<Image>().color = itemColor;
    }


    public ClassItemData CheckIfItemExistOnList(List<ClassItemData> scriptableItemList, ScriptableItem scriptableItem)
    {
        int index = scriptableItemList.FindIndex(item => item.scriptableItem.itemName == scriptableItem.itemName);

        if (index == -1)
        {
            return null;
        }
        else
        {
            return scriptableItemList[index];
        }
    }

    public void AddRemoveInventoryItemInListByScriptableName(string name, int quantity)
    {

        ClassItemData itemFound = GetItemByScriptableName(name, StaticData.inventoryItemList);

        itemFound.quantity += quantity;

        if (itemFound.quantity <= 0)
        {
            ItemManager.Instance.RemoveItemFromListGOFromLoot(itemFound, StaticData.inventoryItemList);
        }

    }

    public ClassItemData GetItemByScriptableName(string name, List<ClassItemData> listOfItems)
    {
        return listOfItems.FirstOrDefault(item => item.scriptableItem.name == name);
    }

    public int GetItemIndexByScriptableName(string name, List<ClassItemData> listOfItems)
    {
        int index = listOfItems.FindIndex(item => item.scriptableItem.itemName == name);

        return index;
    }

    public int GetItemQuantity(string name, List<ClassItemData> listOfItems)
    {

        int quantity = 0;

        int itemIndex = ItemManager.Instance.GetItemIndexByScriptableName("Gold", StaticData.inventoryItemList);
      
        if (itemIndex != -1)
        {
            quantity = listOfItems[itemIndex].quantity;
        }

        return quantity;

    }

    public bool CanPlayerBuy(int priceCost)
    {

        bool canBuy = false;

        int playerGold = ItemManager.Instance.GetItemQuantity("Gold", StaticData.inventoryItemList);
        
        if (playerGold < priceCost)
        {
            canBuy = false;
        }
        else
        {
            canBuy = true;
        }

        return canBuy;

    }
}
