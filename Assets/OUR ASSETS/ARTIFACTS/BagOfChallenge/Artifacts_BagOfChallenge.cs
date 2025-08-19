using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_BagOfChallenge", menuName = "Item/Artifacts/Artifacts_BagOfChallenge")]
public class Artifacts_BagOfChallenge : ScriptableItem
{

    public List<ScriptableItem> scriptableItems;
    int maxPercentage = 25;

    private bool playedCard = false;

    public override void Activate(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {

        if (playedCard)
        {
            return;
        }

        // Define what happens when this ability upgrade is activated

        int chance = Random.Range(0, maxPercentage);


        if (chance <= maxPercentage)
        {

            //get random item
            int randomItem = Random.Range(0, scriptableItems.Count);

            ItemManager.Instance.AddItemOnActivateOrder(this, "Extra " + scriptableItems[randomItem].itemName, false);

            // Default behavior if needed
            ClassItemData classItemTemp = new ClassItemData(scriptableItems[randomItem], classItem.tempValue);
            classItemTemp.customToolTip = "Bag Of Challenge Extra Loot!";

            StaticData.lootItemList.Add(classItemTemp);
        }


        //then add item to loot
        //ItemManager.Instance.AddItemToParent(classItemTemp, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);

    }

    public override void Initialiaze(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        playedCard = false;
    }

    public override void Expired(ClassItemData classItem, CardScriptData cardScriptData, GameObject target)
    {
        if(Combat.Instance.turns == 1)
        {
            playedCard = true;
            ItemManager.Instance.AddItemOnActivateOrder(this, "No Extra Items!" , true);
        }
      
    }
}
