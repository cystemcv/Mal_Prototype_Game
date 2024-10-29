using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void AddItemToInventory(ClassItem classItem)
    {
        // Check if the item already exists in the inventory
        ClassItem existingItem = StaticData.inventory.Find(item => item.scriptableItem == classItem.scriptableItem);

        if (existingItem != null)
        {
            // If the item exists, increase its quantity
            existingItem.quantity += classItem.quantity;
        }
        else
        {
            // If the item doesn't exist, add it to the inventory
            StaticData.inventory.Add(classItem);
        }
    }

    public void RemoveItemFromInventory(ClassItem classItem, int amountToRemove)
    {
        // Find the item in the inventory
        ClassItem existingItem = StaticData.inventory.Find(item => item.scriptableItem == classItem.scriptableItem);

        if (existingItem != null)
        {
            // Decrease the quantity
            existingItem.quantity -= amountToRemove;

            // If the quantity is zero or less, remove the item from the inventory
            if (existingItem.quantity <= 0)
            {
                StaticData.inventory.Remove(existingItem);
            }
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
        }
    }
    public void AddItemToLoot(ClassItem classItem)
    {
        // Check if the item already exists in the inventory
        ClassItem existingItem = StaticData.loot.Find(item => item.scriptableItem == classItem.scriptableItem);

        if (existingItem != null)
        {
            // If the item exists, increase its quantity
            existingItem.quantity += classItem.quantity;
        }
        else
        {
            // If the item doesn't exist, add it to the inventory
            StaticData.loot.Add(classItem);
        }
    }

    public void RemoveItemFromLoot(ClassItem classItem, int amountToRemove)
    {
        // Find the item in the inventory
        ClassItem existingItem = StaticData.loot.Find(item => item.scriptableItem == classItem.scriptableItem);

        if (existingItem != null)
        {
            // Decrease the quantity
            existingItem.quantity -= amountToRemove;

            // If the quantity is zero or less, remove the item from the inventory
            if (existingItem.quantity <= 0)
            {
                StaticData.loot.Remove(existingItem);
            }
        }
        else
        {
            Debug.LogWarning("Item not found in inventory.");
        }
    }

    public void ShowInventory()
    {

        //temporary for testing

        foreach (ScriptableItem scriptableItem in scriptableItemList)
        {

            ClassItem classItem = new ClassItem();
            classItem.scriptableItem = scriptableItem;
            classItem.quantity = Random.Range(0, 20);

            StaticData.inventory.Add(classItem);

        }


        SystemManager.Instance.DestroyAllChildren(UIManager.Instance.inventoryGO);

        UIManager.Instance.inventoryGO.transform.parent.gameObject.SetActive(true);

        //if (StaticData.inventory.Count == 0)
        //{
        //    return;
        //}

        foreach (ClassItem classItem in StaticData.inventory)
        {

            GameObject classItemGO = Instantiate(UIManager.Instance.itemPrefab, UIManager.Instance.inventoryGO.transform.position, Quaternion.identity);
            classItemGO.transform.SetParent(UIManager.Instance.inventoryGO.transform);
            classItemGO.AddComponent<ClassItem>().SetData(classItem);

            classItemGO.GetComponent<Image>().sprite = classItem.scriptableItem.Icon;
            classItemGO.transform.GetChild(0).GetComponent<TMP_Text>().text = classItem.quantity.ToString();
            UIManager.Instance.inventoryItemText.GetComponent<TMP_Text>().text = classItem.scriptableItem.itemName;

        }



    }

    public void HideInventory()
    {

        UIManager.Instance.inventoryGO.transform.parent.gameObject.SetActive(false);
    }
}
