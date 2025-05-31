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

   

    public override void OnButtonClick()
    {
        base.OnButtonClick();

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene
                                                           //hit at least one time if its 0

        // Start the coroutine for each hit
        runner.StartCoroutine(EventEnd());



    }

    public IEnumerator EventEnd()
    {

        MonoBehaviour runner = CombatCardHandler.Instance; // Ensure this is a valid MonoBehaviour in your scene

        yield return runner.StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(UIManager.Instance.shopUI.transform.Find("CardList").gameObject));

        yield return runner.StartCoroutine(ForceOpenEvent());

        yield return null;
    }

    public IEnumerator ForceOpenEvent()
    {
        //open shop ui
        UIManager.Instance.shopUI.SetActive(true);

        //get player gold
        int playerGold = ItemManager.Instance.GetItemQuantity("Gold", StaticData.inventoryItemList);

        UIManager.Instance.shopUI.transform.Find("PlayerGold").GetComponent<TMP_Text>().text = playerGold.ToString();

        //add cards to the shop
        List<ShopCardData> shopCardList = new List<ShopCardData>();

        if (CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopCards.Count <= 0)
        {
            var allowedClasses = new List<MainClass> { MainClass.COMMON };
            var allowedTypes = new List<CardType> { CardType.Attack };
            var allowedRarities = new List<CardRarity> { CardRarity.Common, CardRarity.Rare };

            allowedClasses.Add(StaticData.staticCharacter.mainClass);

            List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, null, null, 7, true);

            //build the shop card list
            foreach (ScriptableCard scriptableCard in cardList)
            {
                ShopCardData shopCard = new ShopCardData();

                shopCard.shopCostItem = GetShopCostCard(scriptableCard);
                shopCard.itemAvailable = true;
                shopCard.scriptableCard = scriptableCard;

                shopCardList.Add(shopCard);

            }

        }
        else
        {
            shopCardList = CombatManager.Instance.planetClicked.GetComponent<RoomScript>().shopCards;
        }



        AssignCardsOnShop(shopCardList);

        yield return null;
    }


    public void AssignCardsOnShop(List<ShopCardData> cardList)
    {

        foreach (ShopCardData shopCardData in cardList)
        {
            CardScript cardScript = new CardScript();
            cardScript.scriptableCard = shopCardData.scriptableCard;
            //instantiate the card
            GameObject card = DeckManager.Instance.InitializeCardPrefab(cardScript, UIManager.Instance.shopUI.transform.Find("CardList").gameObject, false, false);

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

            card.GetComponent<CardListCardEvents>().markedGO = card.transform.GetChild(0).Find("UtilityFront").Find("Marked").gameObject;
            card.GetComponent<CardListCardEvents>().markedGO.SetActive(false);
            card.GetComponent<CardListCardEvents>().scriptableCard = shopCardData.scriptableCard;

            GameObject cardGoldPanel = card.transform.GetChild(0).Find("UtilityFront").Find("GoldPanel").gameObject;
            cardGoldPanel.SetActive(true);

            card.AddComponent<ShopCard>();
            card.GetComponent<ShopCard>().shopCardData = shopCardData;


            if (!shopCardData.itemAvailable)
            {
                card.transform.GetChild(0).Find("UtilityFront").Find("SoldOut").gameObject.SetActive(true);
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = "SOLD";
            }
            else
            {
                cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = card.GetComponent<ShopCard>().shopCardData.shopCostItem.ToString();

                if (ItemManager.Instance.CanPlayerBuy(card.GetComponent<ShopCard>().shopCardData.shopCostItem))
                {
                    cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
                }
                else
                {
                    cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
                }
            }



    
           

        }




    }

    public int GetShopCostCard(ScriptableCard scriptableCard)
    {

        int goldCost = 0;

        if (scriptableCard.cardRarity == SystemManager.CardRarity.Common)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minCommonCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxCommonCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.CardRarity.Rare)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minRareCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxRareCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.CardRarity.Epic)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minEpicCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxEpicCardCost);
        }
        else if (scriptableCard.cardRarity == SystemManager.CardRarity.Legendary)
        {
            goldCost = Random.Range(CustomDungeonGenerator.Instance.galaxyGenerating.minLegendaryCardCost, CustomDungeonGenerator.Instance.galaxyGenerating.maxLegendaryCardCost);
        }

        return goldCost;

    }




}
