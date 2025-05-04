using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Artifacts_BagOfChallenge", menuName = "Item/Artifacts/Artifacts_BagOfChallenge")]
public class Artifacts_BagOfChallenge : ScriptableItem
{

    public List<ScriptableItem> scriptableItems;
    int maxPercentage = 25;

    private bool playedCard = false;

    public override void Activate(ClassItem classItem, CardScript cardScript)
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

            ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Activated! Extra " + scriptableItems[randomItem].itemName, false);

            // Default behavior if needed
            ClassItem classItemTemp = new ClassItem(scriptableItems[randomItem], classItem.tempValue);
            classItemTemp.customToolTip = "Bag Of Challenge Extra Loot!";

            StaticData.lootItemList.Add(classItemTemp);
        }


        //then add item to loot
        //ItemManager.Instance.AddItemToParent(classItemTemp, UIManager.Instance.lootGO, SystemManager.ItemIn.LOOT);

    }

    public override void Initialiaze(ClassItem classItem, CardScript cardScript)
    {
        playedCard = false;
    }

    public override void Expired(ClassItem classItem, CardScript cardScript)
    {
        if(Combat.Instance.turns == 1)
        {
            playedCard = true;
            ItemManager.Instance.AddItemOnActivateOrder(this, this.itemName + " Failed! No Extra Items!" , true);
        }
      
    }
}
