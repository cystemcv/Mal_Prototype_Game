using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassItem : MonoBehaviour
{

    public ScriptableItem scriptableItem;
    public int quantity = 0;
    public string itemID = "";

    public ClassItem()
    {
        //add an id to this scriptableCard, this is in order to identify it by comparisons
        itemID = System.Guid.NewGuid().ToString();
    }

    // Method to set the data
    public void SetData(ClassItem item)
    {
        scriptableItem = item.scriptableItem;
        quantity = item.quantity;
        itemID = "x" + item.itemID.ToString();
    }

}
