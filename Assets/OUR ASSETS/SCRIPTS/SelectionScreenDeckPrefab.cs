using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static UIManager;

public class SelectionScreenDeckPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public ScriptableDeck scriptableDeck;

    public bool onHover = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (StaticData.staticCharacter.startingCards == null)
        {
            return;
        }

        if (scriptableDeck.title == StaticData.staticCharacter.startingCards.title)
        {
            this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
        }
        else
        {
            this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
        }



    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        onHover = true;


        AudioManager.Instance.PlaySfx("UI_Confirm");
        this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorYellow);
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        onHover = false;
        this.gameObject.transform.Find("Outline").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //point this deck
        StaticData.staticCharacter.startingCards = scriptableDeck;
        AudioManager.Instance.PlaySfx("UI_goNext");
    }

    public void ShowDeckCardList()
    {
        List<CardScriptData> cardScriptDataList = new List<CardScriptData>();

        foreach (ScriptableCard scriptableCard in scriptableDeck.deck)
        {
            CardScriptData cardScriptData = new CardScriptData();
            cardScriptData.scriptableCard = scriptableCard;

            cardScriptDataList.Add(cardScriptData);
        }


        OptionsSettings optionsSettings = new OptionsSettings();
        optionsSettings.cardScriptDataList = cardScriptDataList;
        optionsSettings.cardListMode = CardListMode.VIEW;
        optionsSettings.enableCloseButton = true;
        optionsSettings.enableMinSelection = 0;
        optionsSettings.enableMaxSelection = 0;
        optionsSettings.title = scriptableDeck.title;
        //optionsSettings.onConfirmAction = CombatManager.Instance.CardList_AddCardsToDeck;
        optionsSettings.allowClassButtons = false;
        optionsSettings.allowDuplicates = true;
        UIManager.Instance.ShowCardList(optionsSettings);
    }

}
