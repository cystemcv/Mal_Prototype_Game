using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;

public class ItemManager : MonoBehaviour
{

    public static ItemManager Instance;

    public List<ScriptableItem> scriptableItemList;



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

    public void AddItemToParent(ClassItem classItem, GameObject itemParent, SystemManager.ItemIn itemIn)
    {

        bool founditem = false;

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
        gameObject.GetComponent<TooltipContent>().description = uiClassItem.scriptableItem.itemDescription;
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
        gameObject.GetComponent<TooltipContent>().description = "";
    }

    public bool RemoveItemToParent(ClassItem classItem, GameObject itemParent)
    {

        bool founditem = false;

        //if the item is found then increase its quantity
        foreach (Transform child in itemParent.transform)
        {
            ClassItem uiClassItem = child.GetComponent<ClassItem>();

            if (uiClassItem != null && uiClassItem.scriptableItem.itemName == classItem.scriptableItem.itemName)
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
}
