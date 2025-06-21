using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipesData 
{

    public ScriptableCard scriptableCard;
    public ScriptableItem scriptableItem;
    public List<ClassItemData> craftingMaterialsList;
    public string uniqueID = "";

    public CraftingRecipesData()
    {
        uniqueID = System.Guid.NewGuid().ToString();
    }

}
