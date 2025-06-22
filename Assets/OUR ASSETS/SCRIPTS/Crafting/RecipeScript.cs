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
            NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.ERROR, "CANNOT CRAFT!", "Insufficient materials to craft this recipe!");
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
            NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.SUCCESS, "CRAFT SUCCESSFULL!", "Crafted Card '" + craftingRecipesData.scriptableCard.cardName + "' !");

        }
        else
        {
            if (craftingRecipesData.scriptableItem.itemCategory == SystemManager.ItemCategory.ARTIFACT)
            {
                ClassItemData classItemData = new ClassItemData(craftingRecipesData.scriptableItem, 1);
                ItemManager.Instance.AddArtifactItemInList(classItemData);
                NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.SUCCESS, "CRAFT SUCCESSFULL!", "Crafted Artifact '" + craftingRecipesData.scriptableItem.itemName + "' !");
            }
            else
            {
                ClassItemData classItemData = new ClassItemData(craftingRecipesData.scriptableItem, 1);
                ItemManager.Instance.AddCompanionItemInList(classItemData);
                NotificationSystemManager.Instance.ShowNotification(SystemManager.NotificationOperation.SUCCESS, "CRAFT SUCCESSFULL!", "Crafted Companion Upgrade '" + craftingRecipesData.scriptableItem.itemName + "' !");
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
