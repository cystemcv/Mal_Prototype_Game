using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassItemData 
{

    public ScriptableItem scriptableItem;
    public int quantity = 0;
    public string itemID = "";
    public SystemManager.ItemIn itemIn = SystemManager.ItemIn.INVENTORY;

    public int level = 0;
    public int tempValue = 0;

    public string customToolTip = "";

    public ClassItemData(ScriptableItem scriptableItem, int quantity)
    {
        //add an id to this scriptableCard, this is in order to identify it by comparisons
        itemID = System.Guid.NewGuid().ToString();

        this.scriptableItem = scriptableItem;
        this.quantity = quantity;
    }

    public void SetData(ClassItemData item)
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

}
