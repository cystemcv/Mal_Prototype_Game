using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using System.Linq;

public class ItemManager : MonoBehaviour
{

    public static ItemManager Instance;

    public List<ScriptableItem> scriptableItemList;

 
    public List<ScriptableItem> relics;

    public GameObject itemChoosePrefab;

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

    }

    public void Start()
    {
        //foreach (ScriptableItem scriptableItem in scriptableItemList)
        //{

        //    ClassItem classItem = new ClassItem(scriptableItem, Random.Range(0, 100));
        //    classItem.scriptableItem = scriptableItem;
        //    classItem.quantity = Random.Range(0, 20);

        //    //AddItemToParent(classItem, UIManager.Instance.inventoryGO, SystemManager.ItemIn.INVENTORY);
        //    AddItemToParent(classItem, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);
        //    //StaticData.inventory.Add(classItem);

        //}

        UIManager.Instance.lootGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";
        UIManager.Instance.inventoryGO.transform.parent.Find("ItemText").GetComponent<TMP_Text>().text = "";
    }


    public string GenerateNewID()
    {
        return System.Guid.NewGuid().ToString();
    }

    public void AddItemToParent(ClassItem classItem, GameObject itemParent, SystemManager.ItemIn itemIn)
    {

        bool founditem = false;

        //on loot we dont want items to combine
        if (itemIn != SystemManager.ItemIn.LOOT) {

            //if the item is found then increase its quantity
            foreach (Transform child in itemParent.transform)
            {
                ClassItem uiClassItem = child.GetComponent<ClassItem>();

                if (uiClassItem != null && uiClassItem.scriptableItem.itemName == classItem.scriptableItem.itemName)
                {

                    // If the item exists, increase its quantity
                    uiClassItem.quantity += classItem.quantity;

                    if (uiClassItem.quantity >= 999)
                    {
                        uiClassItem.quantity = 999;
                    }


                    UpdateItemUI(child.gameObject, uiClassItem);
                    founditem = true;
                    break;
                }
            }
        }

        //if the item is not found then add it
        if (founditem == false)
        {
            // Find the first empty space
            foreach (Transform child in itemParent.transform)
            {
                ClassItem uiClassItem = child.GetComponent<ClassItem>();

                //find the first empty space
                if (uiClassItem == null)
                {
                    // Update the UI component if needed, e.g., by updating a quantity text
                    ClassItem classItemGO = new ClassItem(classItem.scriptableItem, classItem.quantity);


                    //attach the script
                    child.gameObject.AddComponent<ClassItem>();

                    child.gameObject.GetComponent<ClassItem>().SetData(classItemGO);

                    child.gameObject.GetComponent<ClassItem>().itemIn = itemIn;
                    child.gameObject.GetComponent<ClassItem>().customToolTip = classItem.customToolTip;
                    child.gameObject.GetComponent<ClassItem>().itemID = GenerateNewID();

                    //update the ui
                    UpdateItemUI(child.gameObject, classItemGO);
                    break;
                }

            }
        }


    }

    public void UpdateItemUI(GameObject gameObject, ClassItem uiClassItem)
    {

        //update the icon
        gameObject.transform.Find("Image").gameObject.SetActive(true);
        gameObject.transform.Find("Image").GetComponent<Image>().sprite = uiClassItem.scriptableItem.Icon;

        //updare the quantity
        gameObject.transform.Find("TextParent").gameObject.SetActive(true);
        gameObject.transform.Find("TextParent").Find("Text").GetComponent<TMP_Text>().text = uiClassItem.quantity.ToString();

        //tooltip
        //gameObject.GetComponent<TooltipContent>().description = uiClassItem.scriptableItem.itemDescription;
    }

    public void UpdateRemovedItemUI(GameObject gameObject)
    {
        //update the icon
        gameObject.transform.Find("Image").gameObject.SetActive(false);
        gameObject.transform.Find("Image").GetComponent<Image>().sprite = null;

        //updare the quantity
        gameObject.transform.Find("TextParent").gameObject.SetActive(false);
        gameObject.transform.Find("TextParent").Find("Text").GetComponent<TMP_Text>().text = "";

        //tooltip
        //gameObject.GetComponent<TooltipContent>().description = "";
    }

    public bool RemoveItemToParent(GameObject item, GameObject itemParent)
    {

        ClassItem classItem = item.GetComponent<ClassItem>();


        bool founditem = false;

        //if the item is found then increase its quantity
        foreach (Transform child in itemParent.transform)
        {
            ClassItem uiClassItem = child.GetComponent<ClassItem>();

            if (uiClassItem != null && uiClassItem.itemID == classItem.itemID)
            {

                Destroy(child.GetComponent<ClassItem>());

                UpdateRemovedItemUI(child.gameObject);
                founditem = true;
                break;
            }
        }

        return founditem;


    }

    public void ShowLootParent()
    {
        ShowItemParent(UIManager.Instance.lootGO);
    }

    public void HideLootParent()
    {
        HideItemParent(UIManager.Instance.lootGO);
    }

    public void ShowInventoryParent()
    {
        ShowItemParent(UIManager.Instance.inventoryGO);
    }

    public void HideInventoryParent()
    {
        HideItemParent(UIManager.Instance.inventoryGO);
    }

    public void ShowItemParent(GameObject itemParent)
    {
        itemParent.transform.parent.gameObject.SetActive(true);
    }

    public void HideItemParent(GameObject itemParent)
    {
        itemParent.transform.parent.gameObject.SetActive(false);
    }


    public void ActivateItemList(SystemManager.ActivationType activationType)
    {
        //use all lists of items
        ActivateItems(activationType, StaticData.companionItemList);
        ActivateItems(activationType, StaticData.artifactItemList);
    }

    public void ActivateItems(SystemManager.ActivationType activationType, List<ClassItem> classItems)
    {
        foreach (var item in classItems)
        {
            if (item.scriptableItem.activationType == activationType)
            {
                item.scriptableItem.Activate(item);
            }
        }
    }

    public ClassItem SOItemToClass(ScriptableItem scriptableItem)
    {

        ClassItem classItemTemp = new ClassItem(scriptableItem, 0);
        return classItemTemp;

    }

    public void AddCompanionItemInList(ClassItem classItem)
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

    public bool CheckIfItemMaxLevel(ClassItem classItem)
    {

        bool isMaxLevel = false;

        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.companionItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {
            // If the item exists, find it and increase its level by 1
            ClassItem item = StaticData.companionItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            ) ;

            //stop because its max level
            if (item.level == item.scriptableItem.maxLevel)
            {
                isMaxLevel = true;
            }
        }

        return isMaxLevel;
    }

    public int GetItemLevelFromList(ClassItem classItem)
    {

        int itemLevel = 0;

        // Check if an item with the same scriptableItem name exists in the list
        bool itemExists = StaticData.companionItemList.Any(
            item => item?.scriptableItem?.itemName == classItem.scriptableItem.itemName
        );

        if (itemExists)
        {
            // If the item exists, find it and increase its level by 1
            ClassItem item = StaticData.companionItemList.First(
                item => item.scriptableItem.itemName == classItem.scriptableItem.itemName
            );

            itemLevel = item.level;
        }

        return itemLevel;
    }

    public void PopulateGOObject(GameObject goObject, List<ClassItem> classItems)
    {

        // Get the companion item list
      

        // Loop through companion items and assign each to a child GameObject in goObject
        for (int i = 0; i < classItems.Count && i < goObject.transform.childCount; i++)
        {
            // Get the current item and child
            ClassItem classItem = classItems[i];
            Transform itemPrefab = goObject.transform.GetChild(i);

            // Activate the child GameObject and populate it with the item
            itemPrefab.gameObject.SetActive(true);

            // Assuming itemPrefab has a script (e.g., ItemDisplay) to show the item's details
            ClassItem itemDisplay = itemPrefab.GetComponent<ClassItem>();
            if (itemDisplay != null)
            {

                itemPrefab.GetChild(0).gameObject.SetActive(true);
                itemPrefab.GetChild(1).gameObject.SetActive(true);

                itemDisplay = classItem;

                itemPrefab.GetChild(0).gameObject.GetComponent<Image>().sprite = itemDisplay.scriptableItem.Icon;

                //level or quantity
                if (goObject == UIManager.Instance.inventoryGO)
                {
                    itemPrefab.GetChild(1).Find("Text").gameObject.GetComponent<TMP_Text>().text = itemDisplay.quantity.ToString();
                }
                else
                {
                    itemPrefab.GetChild(1).Find("Text").gameObject.GetComponent<TMP_Text>().text = "Lv:" + itemDisplay.level.ToString();
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
            foreach (Transform itemPrefabChild in itemPrefab)
            {
                itemPrefabChild.gameObject.SetActive(false);
            }
        }

    }

    public void OpenCompanionGO()
    {

        //populate companionGO
        ResetGOObject(UIManager.Instance.companionGO);

        PopulateGOObject(UIManager.Instance.companionGO, StaticData.companionItemList);

        UIManager.Instance.companionGO.SetActive(true);
        UIManager.Instance.inventoryGO.SetActive(false);
        UIManager.Instance.artifactGO.SetActive(false);
    }

    public void OpenInventoryGO()
    {

        UIManager.Instance.inventoryGO.SetActive(true);
        UIManager.Instance.companionGO.SetActive(false);
        UIManager.Instance.artifactGO.SetActive(false);

    }

    public void OpenArtifactGO()
    {

        //populate companionGO
        ResetGOObject(UIManager.Instance.artifactGO);

        PopulateGOObject(UIManager.Instance.companionGO, StaticData.artifactItemList);

        UIManager.Instance.artifactGO.SetActive(true);
        UIManager.Instance.companionGO.SetActive(false);
        UIManager.Instance.inventoryGO.SetActive(false);
    }

}
