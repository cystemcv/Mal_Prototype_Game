using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PickPocket", menuName = "Item/CompanionPassives/PickPocket")]
public class PickPocket : ScriptableItem
{

    public ScriptableItem goldSO;

    public override void Activate(ClassItem classItem)
    {
        // Define what happens when this ability upgrade is activated

        int chance = Random.Range(0, 100);

        int gold = 0;

        if (chance <= 100)
        {
            gold = 10 * classItem.level;
        }
        classItem.tempValue = gold;

        // Default behavior if needed
        ClassItem classItemTemp = new ClassItem(goldSO, classItem.tempValue);
        classItemTemp.customToolTip = "EXTRA GOLD! (Received from Pick Pocket!)";

        //then add item to loot
        ItemManager.Instance.AddItemToParent(classItemTemp, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);

    }
}
