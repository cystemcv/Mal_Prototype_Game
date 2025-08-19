using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thief_PickPocket", menuName = "Item/CompanionPassives/Thief_PickPocket")]
public class Thief_PickPocket : ScriptableItem
{

    public ScriptableItem goldSO;
    public int chancePerc = 100;
    public int goldGained = 5;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        // Define what happens when this ability upgrade is activated

        int chance = Random.Range(0, 100);

        int gold = 0;

        if (chance <= chancePerc)
        {
            gold = goldGained * classItem.level;
            ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated! (" + this.itemDescription + ") Extra +" + gold, false);
        }
        classItem.tempValue = gold;

        // Default behavior if needed
        ClassItemData classItemTemp = new ClassItemData(goldSO, classItem.tempValue);
        classItemTemp.customToolTip = "EXTRA GOLD! (Received from Pick Pocket!)";

        StaticData.lootItemList.Add(classItemTemp);

        //then add item to loot
        //ItemManager.Instance.AddItemToParent(classItemTemp, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);

    }
}
