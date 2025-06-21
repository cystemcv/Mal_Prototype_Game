using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeScript : MonoBehaviour
{

    public CraftingRecipesData craftingRecipesData;

    public void CraftRecipe()
    {

        //check if recipe can be crafted
        if (!CraftingManager.Instance.CanRecipeBeCrafted(craftingRecipesData))
        {
            Debug.Log("CANNOT CRAFT!");
            return;
        }

        //check for artifact

        //check for max level companion

        //REMOVE ITEMS
        CraftingManager.Instance.RemoveItemsFromInventory(craftingRecipesData);

        //create card/artifact/companion upgrade
        if (craftingRecipesData.scriptableCard != null)
        {

            //add card on deck
            DeckManager.Instance.AddCardOnDeck(craftingRecipesData.scriptableCard, 0);

        }
        else
        {
            if (craftingRecipesData.scriptableItem.itemCategory == SystemManager.ItemCategory.ARTIFACT)
            {
                ClassItemData classItemData = new ClassItemData(craftingRecipesData.scriptableItem, 1);
                ItemManager.Instance.AddArtifactItemInList(classItemData);
            }
            else
            {
                ClassItemData classItemData = new ClassItemData(craftingRecipesData.scriptableItem, 1);
                ItemManager.Instance.AddCompanionItemInList(classItemData);
            }

        }

        //remove recipe
        CraftingManager.Instance.RemoveRecipeFromList(craftingRecipesData,StaticData.craftingRecipesDataList);

        //destroy the ui
        Destroy(this.gameObject);

        CraftingManager.Instance.FixCraftingMaterials();

        CraftingManager.Instance.UpdateCraftingRecipeNumberUI();

    }

    public void DeleteRecipe()
    {
        //remove recipe
        CraftingManager.Instance.RemoveRecipeFromList(craftingRecipesData, StaticData.craftingRecipesDataList);

        //destroy the ui
        Destroy(this.gameObject);

        CraftingManager.Instance.UpdateCraftingRecipeNumberUI();
    }



}
