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

        yield return runner.StartCoroutine(ForceOpenEvent());

        //yield return runner.StartCoroutine(UIManager.Instance.EndEvent(finalWording));

        yield return null;
    }

    public IEnumerator ForceOpenEvent()
    {
        //open shop ui
        UIManager.Instance.shopUI.SetActive(true);

        //player gold
        int playerGoldIndex = ItemManager.Instance.GetItemIndexByScriptableName("Gold",StaticData.inventoryItemList);
        int playerGoldInt = 0;
        ClassItem playerGold = StaticData.inventoryItemList[playerGoldIndex];

        if (playerGoldIndex != -1)
        {
            playerGoldInt = playerGold.quantity;
        }
  

        UIManager.Instance.shopUI.transform.Find("PlayerGold").GetComponent<TMP_Text>().text = playerGoldInt.ToString();

        //add cards to the shop
        var allowedClasses = new List<MainClass> { MainClass.COMMON };
        var allowedTypes = new List<CardType> { CardType.Attack };
        var allowedRarities = new List<CardRarity> { CardRarity.Common, CardRarity.Rare };

        allowedClasses.Add(StaticData.staticCharacter.mainClass);

        List<ScriptableCard> cardList = CardListManager.Instance.ChooseCards(allowedClasses, null, null, null, 7, true);

        AssignCardsOnShop(cardList);

        yield return null;
    }


    public void AssignCardsOnShop(List<ScriptableCard> cardList)
    {

        foreach (ScriptableCard scriptableCard in cardList)
        {
            CardScript cardScript = new CardScript();
            cardScript.scriptableCard = scriptableCard;
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
            card.GetComponent<CardListCardEvents>().scriptableCard = scriptableCard;

            GameObject cardGoldPanel = card.transform.GetChild(0).Find("UtilityFront").Find("GoldPanel").gameObject;
            cardGoldPanel.SetActive(true);
            cardGoldPanel.transform.Find("Text").GetComponent<TMP_Text>().text = Random.Range(10, 999).ToString();

        }




    }


}
