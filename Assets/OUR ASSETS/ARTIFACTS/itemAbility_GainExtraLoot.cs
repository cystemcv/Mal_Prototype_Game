using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "itemAbility_GainExtraLoot", menuName = "Item/ItemAbility/itemAbility_GainExtraLoot")]
public class itemAbility_GainExtraLoot : ScriptableItem
{
    [Title("UNIQUE ITEM ABILITY")]
    public enum MethodToUse { CHOOSERANDOM, LOOPALL }
    public MethodToUse methodToUse;

    [TextArea] public string customToolTip = "";

    public List<ExtraItemProperties> itemExtraPropertiesList;

    public override void Activate(ClassItem classItem, CardScript cardScript)
    {
        // Check if only one random item should be chosen
        if (methodToUse == MethodToUse.CHOOSERANDOM)
        {
            // Choose a single random item from the list
            if (itemExtraPropertiesList.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, itemExtraPropertiesList.Count);
         

                // Add item to loot
                ClassItem classItemTemp = new ClassItem(itemExtraPropertiesList[randomIndex].scriptableItem, 1)
                {
                    customToolTip = customToolTip
                };

                StaticData.lootItemList.Add(classItemTemp);
            }
        }
        else
        {
            // Loop through all items if LOOPALL is selected
            foreach (ExtraItemProperties extraItemProperties in itemExtraPropertiesList)
            {
                ProcessItem(extraItemProperties);
            }
        }
    }

    private void ProcessItem(ExtraItemProperties extraItemProperties)
    {
        // Check chance to get item
        int chanceToGet = UnityEngine.Random.Range(0, 100);
        if (chanceToGet > extraItemProperties.chanceToGet) return;

        // Get the quantity within min and max range
        int quantity = UnityEngine.Random.Range(extraItemProperties.minQuantity, extraItemProperties.maxQuantity);

        // Add item to loot
        ClassItem classItemTemp = new ClassItem(extraItemProperties.scriptableItem, quantity)
        {
            customToolTip = customToolTip
        };

        StaticData.lootItemList.Add(classItemTemp);
    }

    [Serializable]
    public class ExtraItemProperties
    {
        [Title("ITEM"), GUIColor("orange")]
        public ScriptableItem scriptableItem;

        [Title("PROPERTIES")]
        [ShowIf("@UnityEditor.Selection.activeObject is itemAbility_GainExtraLoot && ((itemAbility_GainExtraLoot)UnityEditor.Selection.activeObject).methodToUse == itemAbility_GainExtraLoot.MethodToUse.LOOPALL")]
        [Range(0, 100)]
        public int chanceToGet = 0;
        public int minQuantity = 0;
        public int maxQuantity = 0;
    }
}
