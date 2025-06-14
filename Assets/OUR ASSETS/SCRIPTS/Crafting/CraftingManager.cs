using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


        //companion
        if (randomChance <= 10)
        {
            var allowedTypes = new List<ItemCategory> { ItemCategory.COMPANIONITEM };
            var allowedRarities = new List<Rarity> {};
            allowedRarities.Add(rarityCreation.rarity);

            List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(StaticData.staticScriptableCompanion.companionItemList, allowedTypes, allowedRarities, 1, false);
            craftingRecipesData.scriptableItem = itemList[0];
        }

        randomChance = UnityEngine.Random.Range(0, 100);
        //artifact
        if (randomChance <= 10)
        {
            var allowedTypes = new List<ItemCategory> { ItemCategory.ARTIFACT };
            var allowedRarities = new List<Rarity> { };
            allowedRarities.Add(rarityCreation.rarity);

            List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(ItemManager.Instance.artifactPoolList, allowedTypes, allowedRarities, 1, false);
            craftingRecipesData.scriptableItem = itemList[0];
        }

        //card
        if (randomChance <= 100)
        {
            var allowedClasses = new List<MainClass> { MainClass.COMMON };
            var allowedRarities = new List<Rarity> { };
            allowedRarities.Add(rarityCreation.rarity);

            allowedClasses.Add(StaticData.staticCharacter.mainClass);

            List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, allowedRarities, null, 7, true);
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


}
