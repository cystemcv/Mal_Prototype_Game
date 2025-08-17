using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SystemManager;

public class CraftingManager : MonoBehaviour
{

    [Serializable]
    public struct RarityCreation
    {

        public SystemManager.Rarity rarity;
        public int itemsToGenerate;

    }

    public static CraftingManager Instance;
    public int recipeListLimit = 30;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        CraftingRecipesData craftingRecipesData = CreateRecipe();

        string test = "";

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RarityCreation GetRarity()
    {

        RarityCreation rarityCreation;

        int randomChance = UnityEngine.Random.Range(0, 100);

        //legendary
        if (randomChance <= 2)
        {
            rarityCreation.rarity = SystemManager.Rarity.Legendary;
            rarityCreation.itemsToGenerate = 4;
            return rarityCreation;
        }

        randomChance = UnityEngine.Random.Range(0, 100);

        //epic
        if (randomChance <= 6)
        {
            rarityCreation.rarity = SystemManager.Rarity.Epic;
            rarityCreation.itemsToGenerate = 4;
            return rarityCreation;
        }

        randomChance = UnityEngine.Random.Range(0, 100);

        //rare
        if (randomChance <= 20)
        {
            rarityCreation.rarity = SystemManager.Rarity.Rare;
            rarityCreation.itemsToGenerate = 4;
            return rarityCreation;
        }

        rarityCreation.rarity = SystemManager.Rarity.Common;
        rarityCreation.itemsToGenerate = 4;
        return rarityCreation;

    }

    public CraftingRecipesData CreateRecipe()
    {
        CraftingRecipesData craftingRecipesData = new CraftingRecipesData();

        //first should decide if its a card/artifact or companion

        int randomChance = UnityEngine.Random.Range(0, 100);

        //get the rarity
        RarityCreation rarityCreation = GetRarity();

        craftingRecipesData.craftingMaterialsList = CreateRecipeItems(rarityCreation);

        bool found = false;


        ////companion
        //if (randomChance <= 30 && !found)
        //{
        //    var allowedTypes = new List<ItemCategory> { ItemCategory.COMPANIONITEM };
        //    var allowedRarities = new List<Rarity> {};
        //    allowedRarities.Add(rarityCreation.rarity);

        //    List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(StaticData.staticScriptableCompanion.companionItemList, allowedTypes, null, 1, false);

        //    if (itemList[0] !=  null)
        //    {
        //        craftingRecipesData.scriptableItem = itemList[0];
        //        found = true;
        //    }
       
        //}

        randomChance = UnityEngine.Random.Range(0, 100);
        //artifact
        if (randomChance <= 30 && !found)
        {
            var allowedTypes = new List<ItemCategory> { ItemCategory.ARTIFACT };
            var allowedRarities = new List<Rarity> { };
            allowedRarities.Add(rarityCreation.rarity);

            List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(ItemManager.Instance.artifactPoolList, allowedTypes, null, 1, false);

            if (itemList[0] != null)
            {
                craftingRecipesData.scriptableItem = itemList[0];
                found = true;
            }
        }

        //card
        if (randomChance <= 100 && !found)
        {
            var allowedClasses = new List<MainClass> { MainClass.COMMON };
            var allowedRarities = new List<Rarity> { };
            allowedRarities.Add(rarityCreation.rarity);

            allowedClasses.Add(StaticData.staticCharacter.mainClass);

            List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, allowedRarities, null, null, 7, true);
            craftingRecipesData.scriptableCard = cardList[0];
        }


        return craftingRecipesData;

    }


    public List<ClassItemData> CreateRecipeItems(RarityCreation rarityCreation)
    {

        List<ClassItemData> listClassItemData = new List<ClassItemData>();

      
        var allowedRarities = new List<Rarity> {  };
        allowedRarities.Add(rarityCreation.rarity);

        List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(ItemManager.Instance.scriptableItemPoolList, null, allowedRarities, rarityCreation.itemsToGenerate, false);

        //build the artifact card list
        foreach (ScriptableItem scriptableItem in itemList)
        {
            ClassItemData classItemData = new ClassItemData(scriptableItem, UnityEngine.Random.Range(1,30));


            listClassItemData.Add(classItemData);

        }

        return listClassItemData;


    }

    public void OpenCraftingUI(string mode)
    {

        //open the UI for crafting
        UIManager.Instance.craftingPanelUI.SetActive(true);
        FeedbackManager.Instance.PlayOnTarget(UIManager.Instance.craftingPanelUI.transform, FeedbackManager.Instance.mm_OpenPanel_Prefab);

        //testing crafting

        //for (int i = 0; i < recipeListLimit; i++)
        //{
        //    CraftingRecipesData craftingRecipesData = CreateRecipe();
        //    StaticData.craftingRecipesDataList.Add(craftingRecipesData);
        //}


        //generate all the crafting UI
        SystemManager.Instance.DestroyAllChildren(UIManager.Instance.craftingPanelUI_content);

        foreach (CraftingRecipesData data in StaticData.craftingRecipesDataList)
        {
            GameObject recipeGO = Instantiate(UIManager.Instance.recipeUIPrefab, UIManager.Instance.craftingPanelUI_content.transform.position, Quaternion.identity);
            recipeGO.transform.SetParent(UIManager.Instance.craftingPanelUI_content.transform);

            recipeGO.transform.localScale = new Vector3(1f, 1f, 1f);

            //recipeGO.AddComponent<RecipeScript>();
            recipeGO.GetComponent<RecipeScript>().craftingRecipesData = data;


            GameObject craftingReward = recipeGO.transform.Find("CraftingReward").gameObject;
            GameObject itemListMaterials = recipeGO.transform.Find("ItemList").gameObject;
            GameObject craftButtons = recipeGO.transform.Find("Buttons").gameObject;

            SystemManager.Instance.DestroyAllChildren(craftingReward);
            SystemManager.Instance.DestroyAllChildren(itemListMaterials);

            //then means card
            if (data.scriptableCard != null)
            {
                CreateRecipeCard(data, craftingReward);

            }
            else
            {
                CreateRecipeArtifactCompanion(data, craftingReward);
            }


            //create items
            CreateRecipeItems(data,itemListMaterials);

            if (mode != "EDIT")
            {

                //disable the buttons
                foreach(Transform button in craftButtons.transform)
                {
                    button.GetComponent<ButtonManager>().Interactable(false);
                }

            }

        }

        CraftingManager.Instance.UpdateCraftingRecipeNumberUI();

        //



    }


    public void CloseCraftingUI()
    {
        UIManager.Instance.craftingPanelUI.SetActive(false);
    }

    public bool CanRecipeBeCrafted(CraftingRecipesData craftingRecipesData)
    {

        bool canCraft = true;


        foreach (ClassItemData itemData in craftingRecipesData.craftingMaterialsList)
        {

            //get item quantity and compare
            int playerItemQuantity = ItemManager.Instance.GetItemQuantity(itemData.scriptableItem.itemName, StaticData.inventoryItemList);

            //if not enough then break loop...cannot craft
            if (playerItemQuantity < itemData.quantity)
            {
                canCraft = false;
                break;
            }

        }

        return canCraft;

    }

    public void RemoveItemsFromInventory(CraftingRecipesData craftingRecipesData)
    {

        foreach (ClassItemData itemData in craftingRecipesData.craftingMaterialsList)
        {
            //remobve based on the recipe quantity
            ItemManager.Instance.AddRemoveInventoryItemInListByScriptableName(itemData.scriptableItem.itemName, itemData.quantity * -1);
        }


    }

    public void RemoveRecipeFromList(CraftingRecipesData craftingRecipesData, List<CraftingRecipesData> craftingRecipesDatas)
    {
        // Find the index of the item with the same itemID
        int indexToRemove = craftingRecipesDatas.FindIndex(item => item.uniqueID == craftingRecipesData.uniqueID);

        // If the item is found, remove it at the specified index
        if (indexToRemove != -1)
        {
            craftingRecipesDatas.RemoveAt(indexToRemove);
        }

    }


    public void FixCraftingMaterials()
    {

        foreach (Transform recipeGO in UIManager.Instance.craftingPanelUI_content.transform)
        {

            GameObject itemListMaterials = recipeGO.transform.Find("ItemList").gameObject;
            SystemManager.Instance.DestroyAllChildren(itemListMaterials);

            CreateRecipeItems(recipeGO.GetComponent<RecipeScript>().craftingRecipesData, itemListMaterials);

        }

    }

    public void CreateRecipeArtifactCompanion(CraftingRecipesData data, GameObject craftingReward)
    {
        //instantiate the artifact
        GameObject artifact = Instantiate(
  ItemManager.Instance.itemChoosePrefab,
  craftingReward.transform.position,
  Quaternion.identity);

        //set it as a child of the parent
        artifact.transform.SetParent(craftingReward.transform);

        artifact.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);


        //ui
        artifact.transform.Find("Icon").GetComponent<Image>().sprite = data.scriptableItem.Icon;

        //itemPrefab.transform.Find("Title").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemName;
        artifact.transform.Find("Title").GetComponent<TMP_Text>().text = data.scriptableItem.itemName;
        //itemPrefab.transform.Find("Description").GetComponent<TMP_Text>().text = itemClassTemp.scriptableItem.itemDescription;
        artifact.transform.Find("Description").GetComponent<TMP_Text>().text = data.scriptableItem.itemDescription;

        artifact.transform.Find("ItemLevel").gameObject.SetActive(false);

        //check if player already own this artifact
        ClassItemData itemExist = ItemManager.Instance.CheckIfItemExistOnList(StaticData.artifactItemList, data.scriptableItem);
    }

    public void CreateRecipeCard(CraftingRecipesData data, GameObject craftingReward)
    {
        CardScriptData cardScriptData = new CardScriptData();
        cardScriptData.scriptableCard = data.scriptableCard;
        //instantiate the card
        GameObject card = DeckManager.Instance.InitializeCardPrefab(cardScriptData, craftingReward, false, false);

        //do extra stuff on card
        Destroy(card.GetComponent<CanvasScaler>());
        Destroy(card.GetComponent<GraphicRaycaster>());
        Destroy(card.GetComponent<Canvas>());

        //disable scripts not needed
        card.GetComponent<CardScript>().enabled = false;
        card.GetComponent<CardEvents>().enabled = false;

        //enable scripts needed
        card.GetComponent<CardListCardEvents>().enabled = true;
        card.GetComponent<Button>().enabled = true;
        card.GetComponent<CustomButton>().enabled = true;

        card.GetComponent<CardListCardEvents>().markedGO = card.transform.Find("Panel").Find("UtilityFront").Find("Marked").gameObject;
        card.GetComponent<CardListCardEvents>().markedGO.SetActive(false);
        card.GetComponent<CardListCardEvents>().markedNumberGO = card.transform.Find("Panel").Find("UtilityFront").Find("MarkedNumber").gameObject;
        card.GetComponent<CardListCardEvents>().markedNumberGO.SetActive(false);
        card.GetComponent<CardListCardEvents>().scriptableCard = data.scriptableCard;
    }


    public void CreateRecipeItems(CraftingRecipesData data, GameObject itemListMaterials)
    {

        //create items

        foreach (ClassItemData itemData in data.craftingMaterialsList)
        {

            //instantiate the artifact
            GameObject item = Instantiate(
      ItemManager.Instance.itemPrefab,
      itemListMaterials.transform.position,
      Quaternion.identity);

            //set it as a child of the parent
            item.transform.SetParent(itemListMaterials.transform);

            item.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            //remove
            Destroy(item.GetComponent<ClassItem>());


            //ui
            item.transform.Find("Image").GetComponent<Image>().sprite = itemData.scriptableItem.Icon;

            //find out how many the player has
            int playerItemQuantity = ItemManager.Instance.GetItemQuantity(itemData.scriptableItem.itemName, StaticData.inventoryItemList);

            item.transform.Find("Text").GetComponent<TMP_Text>().text = playerItemQuantity.ToString() + "/" + itemData.quantity.ToString();

            if (playerItemQuantity < itemData.quantity)
            {
                item.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
            }

            item.GetComponent<TooltipContent>().description = "<color=yellow>" + itemData.scriptableItem.itemName + "</color> : " + itemData.scriptableItem.itemDescription;

        }


    }

    public void UpdateCraftingRecipeNumberUI()
    {

        //get the text UI
        TMP_Text recipeNumberUI = UIManager.Instance.craftingPanelUI.transform.Find("RecipeNumber").GetComponent<TMP_Text>();

        recipeNumberUI.text = StaticData.craftingRecipesDataList.Count + "/" + recipeListLimit;

    }

}
