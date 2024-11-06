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

    public List<ScriptableItem> companionItemList;
    public List<ScriptableItem> relics;

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
        ActivateItems(activationType, StaticData.relics);
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
            StaticData.companionItemList.Add(classItem);
        }
    }


}
