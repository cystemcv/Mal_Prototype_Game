using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SystemManager;

[CreateAssetMenu(fileName = "Event_Button_Shop", menuName = "Events/ScriptableButtonEvent/Event_Button_Shop")]
public class Event_Button_Shop : ScriptableButtonEvent
{
    [TextArea(1, 20)]
    public string finalWording = "";



    public override void OnButtonClick(GameObject eventButton)
    {
        base.OnButtonClick(eventButton);

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());



    }

    public IEnumerator EventEnd()
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        yield return runner.StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(UIManager.Instance.shopUI.transform.Find("CardList").gameObject));

        yield return runner.StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(UIManager.Instance.shopUI.transform.Find("ArtifactsList").gameObject));

        yield return runner.StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(UIManager.Instance.shopUI.transform.Find("ItemList").gameObject));

        yield return runner.StartCoroutine(ForceOpenEvent());

        yield return null;
    }

    public IEnumerator ForceOpenEvent()
    {
        //open shop ui
        UIManager.Instance.shopUI.SetActive(true);

        AddPlayerGold();

        AssignCards();

        AssignArtifacts();

        AssignItems();

        yield return null;
    }

    public void AddPlayerGold()
    {
        //get player gold
        int playerGold = ItemManager.Instance.GetItemQuantity("Gold", StaticData.inventoryItemList);

        UIManager.Instance.shopUI.transform.Find("PlayerGold").GetComponent<TMP_Text>().text = playerGold.ToString();

    }

    public void AssignItems()
    {
        //add cards to the shop
        List<ShopData> shopItemList = new List<ShopData>();

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopArtifacts.Count <= 0)
        {

            var allowedTypes = new List<ItemCategory> { ItemCategory.RESOURCE, ItemCategory.CONSUMABLE };
            var allowedRarities = new List<Rarity> { Rarity.Common, Rarity.Rare };

            List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(ItemManager.Instance.scriptableItemPoolList, allowedTypes, null, 8, true);

            //build the artifact card list
            foreach (ScriptableItem scriptableItem in itemList)
            {
                ShopData shopData = new ShopData();

                shopData.shopQuantityItem = Random.Range(1, 10);
                shopData.shopCostItem = GetShopCostItem(scriptableItem) * shopData.shopQuantityItem;
                shopData.itemAvailable = true;
                shopData.scriptableItem = scriptableItem;

                shopItemList.Add(shopData);

            }

        }
        else
        {
            shopItemList = CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopItems;
        }



        AssignItemsOnShop(shopItemList);

    }

    public void AssignItemsOnShop(List<ShopData> itemList)
    {

        GameObject parent = UIManager.Instance.shopUI.transform.Find("ItemList").gameObject;

        foreach (ShopData shopData in itemList)
        {

            //instantiate the artifact
            GameObject item = Instantiate(
      ItemManager.Instance.itemPrefab,
      parent.transform.position,
      Quaternion.identity);

            //set it as a child of the parent
            item.transform.SetParent(parent.transform);

            item.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

 

            //remove
            Destroy(item.GetComponent<ClassItem>());

            item.AddComponent<ShopItem>();
            item.GetComponent<ShopItem>().shopData = shopData;

            //ui
            item.transform.Find("Image").GetComponent<Image>().sprite = shopData.scriptableItem.Icon;

            item.transform.Find("Text").GetComponent<TMP_Text>().text = shopData.shopQuantityItem.ToString();

            item.GetComponent<TooltipContent>().description = "<color=yellow>" + shopData.scriptableItem.itemName + "</color> : " + shopData.scriptableItem.itemDescription;

            UIManager.Instance.ItemAvailability(shopData, item);


        }

    }

    public void AssignArtifacts()
    {
        //add cards to the shop
        List<ShopData> shopArtifactList = new List<ShopData>();

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopArtifacts.Count <= 0)
        {

            var allowedTypes = new List<ItemCategory> { ItemCategory.ARTIFACT };
            var allowedRarities = new List<Rarity> { Rarity.Common, Rarity.Rare };

            List<ScriptableItem> itemList = ItemManager.Instance.ChooseItems(ItemManager.Instance.artifactPoolList, allowedTypes, null, 3, false);

            //build the artifact card list
            foreach (ScriptableItem scriptableItem in itemList)
            {
                ShopData shopData = new ShopData();

                shopData.shopCostItem = GetShopCostArtifact(scriptableItem);
                shopData.itemAvailable = true;
                shopData.scriptableItem = scriptableItem;

                shopArtifactList.Add(shopData);



            }

        }
        else
        {
            shopArtifactList = CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopArtifacts;
        }



        AssignArtifactsOnShop(shopArtifactList);

    }

    public void AssignArtifactsOnShop(List<ShopData> artifactsList)
    {

        GameObject parent = UIManager.Instance.shopUI.transform.Find("ArtifactsList").gameObject;

        foreach (ShopData shopData in artifactsList)
        {

            ClassItemData classItemData = new ClassItemData(shopData.scriptableItem, 0);

            //instantiate the card
            GameObject artifact = DeckManager.Instance.InitializeCardArtifactPrefab(classItemData, parent, false, false);

            //do extra stuff on card
            Destroy(artifact.GetComponent<CanvasScaler>());
            Destroy(artifact.GetComponent<GraphicRaycaster>());
            Destroy(artifact.GetComponent<Canvas>());

            //disable scripts not needed
            artifact.GetComponent<CardScript>().enabled = false;
            artifact.GetComponent<CardEvents>().enabled = false;

            //enable scripts needed
            artifact.GetComponent<CardListCardEvents>().enabled = false;
            artifact.GetComponent<Button>().enabled = true;
            artifact.GetComponent<CustomButton>().enabled = true;
            artifact.GetComponent<ItemChoiceClass>().enabled = false;


            artifact.AddComponent<ShopArtifact>();
            artifact.GetComponent<ShopArtifact>().shopData = shopData;


            UIManager.Instance.ArtifactAvailability(shopData, artifact);


        }




    }



    public void AssignCards()
    {
        //add cards to the shop
        List<ShopData> shopCardList = new List<ShopData>();

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopCards.Count <= 0)
        {
            var allowedClasses = new List<MainClass> { MainClass.COMMON };
            var allowedTypes = new List<CardType> { CardType.Attack };
            var allowedRarities = new List<Rarity> { Rarity.Common, Rarity.Rare, Rarity.Epic, Rarity.Legendary };

            allowedClasses.Add(StaticData.staticCharacter.mainClass);

            List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, null, null, null, 6, true);

            //build the shop card list
            foreach (ScriptableCard scriptableCard in cardList)
            {
                ShopData shopData = new ShopData();

                shopData.shopCostItem = GetShopCostCard(scriptableCard);
                shopData.itemAvailable = true;
                shopData.scriptableCard = scriptableCard;

                shopCardList.Add(shopData);

            }

        }
        else
        {
            shopCardList = CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopCards;
        }



        AssignCardsOnShop(shopCardList);

    }


    public void AssignCardsOnShop(List<ShopData> cardList)
    {

        foreach (ShopData shopData in cardList)
        {
            CardScriptData cardScriptData = new CardScriptData();
            cardScriptData.scriptableCard = shopData.scriptableCard;
            //instantiate the card
            GameObject card = DeckManager.Instance.InitializeCardPrefab(cardScriptData, UIManager.Instance.shopUI.transform.Find("CardList").gameObject, false, false);

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
            card.GetComponent<CardListCardEvents>().scriptableCard = shopData.scriptableCard;

            card.AddComponent<ShopCard>();
            card.GetComponent<ShopCard>().shopData = shopData;

            UIManager.Instance.CardAvailability(shopData, card);

        }

    }



    public int GetShopCostCard(ScriptableCard scriptableCard)
    {

        int goldCost = 0;

        if (scriptableCard.cardRarity == SystemManager.Rarity.Common)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minCommonCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxCommonCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Rare)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minRareCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxRareCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Epic)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minEpicCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxEpicCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Legendary)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minLegendaryCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxLegendaryCardCost);
        }

        return goldCost;

    }

    public int GetShopCostArtifact(ScriptableItem scriptableItem)
    {

        int goldCost = 0;

        if (scriptableItem.itemRarity == SystemManager.Rarity.Common)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minCommonArtifactCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxCommonArtifactCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Rare)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minRareArtifactCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxRareArtifactCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Epic)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minEpicArtifactCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxEpicArtifactCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Legendary)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minLegendaryArtifactCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxLegendaryArtifactCost);
        }

        return goldCost;

    }

    public int GetShopCostItem(ScriptableItem scriptableItem)
    {

        int goldCost = 0;

        if (scriptableItem.itemRarity == SystemManager.Rarity.Common)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minCommonItemCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxCommonItemCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Rare)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minRareItemCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxRareItemCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Epic)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minEpicItemCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxEpicItemCost);
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Legendary)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minLegendaryItemCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxLegendaryItemCost);
        }

        return goldCost;

    }





}
