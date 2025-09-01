using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SystemManager;

public class CardListManager : MonoBehaviour
{
    public static CardListManager Instance;

    public Sprite commonBg;
    public Sprite rareBg;
    public Sprite epicBg;
    public Sprite legendaryBg;
    public Sprite curseBg;

    public Sprite artifactCommonBg;
    public Sprite artifactRareBg;
    public Sprite artifactEpicBg;
    public Sprite artifactLegendaryBg;
    public Sprite artifactCurseBg;

    public Sprite iconAngelClass;
    public Sprite iconSummonerClass;
    public Sprite iconMonsterClass;
    public Sprite iconCommonClass;
    public Sprite iconCurseClass;
    public Sprite iconStatusClass;

    public GameObject cardPrefab;
    public GameObject cardListTabButton;
    public GameObject cardListTabButtonParent;
    public List<GameObject> cardListTabButtonList = new List<GameObject>();

    //classes card pool
    [FoldoutGroup("CARD POOLS")]
    public List<CardPoolList> cardPoolLists;

    public int cardsToChooseLimit = 3;

    [Serializable]
    public class CardPoolList
    {
        [Title("CONDITION"), GUIColor("orange")]
        public List<ScriptableCard> scriptableCards;
        [Title("CLASS")]
        public SystemManager.MainClass mainClass = SystemManager.MainClass.COMMON;
    }


    //card abilities prefabs
    public GameObject cardPrefabShield;


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

    public Color GetClassColor(SystemManager.MainClass mainClass)
    {
        Color color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorWhite);

        if (mainClass == SystemManager.MainClass.MONSTER)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorRed);
        }
        else if (mainClass == SystemManager.MainClass.ANGEL)
        {
            color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorBlue);
        }

        return color;
    }

    //HOW TO USE
    //var allowedClasses = new List<MainClass> { MainClass.MONSTER, MainClass.SUPPORT };
    //var allowedTypes = new List<CardType> { CardType.Attack, CardType.Magic };
    //var allowedRarities = new List<CardRarity> { CardRarity.Common, CardRarity.Rare };

    //List<ScriptableCard> result = GetCardList(allowedClasses, allowedTypes, allowedRarities);

    public List<ScriptableCard> GetCardList(
        List<MainClass> allowedMainClasses = null,
        List<CardType> allowedCardTypes = null,
        List<Rarity> allowedCardRarities = null,
        int? allowedPrimaryManaCost = null,
        string cardNameKeyword = null)
    {
        return CardListManager.Instance.cardPoolLists
            .Where(pool =>
                allowedMainClasses == null || allowedMainClasses.Count == 0 ||
                allowedMainClasses.Contains(pool.mainClass))
            .SelectMany(pool => pool.scriptableCards)
            .Where(card =>
                (allowedCardTypes == null || allowedCardTypes.Count == 0 || allowedCardTypes.Contains(card.cardType)) &&
                (allowedCardRarities == null || allowedCardRarities.Count == 0 || allowedCardRarities.Contains(card.cardRarity)) &&
                (!allowedPrimaryManaCost.HasValue || card.primaryManaCost == allowedPrimaryManaCost.Value) &&
                (string.IsNullOrEmpty(cardNameKeyword) ||
                 (card.cardName != null &&
                  card.cardName.IndexOf(cardNameKeyword, StringComparison.OrdinalIgnoreCase) >= 0)))
            .ToList();
    }



    public List<ScriptableCard> ChooseCards(
        List<MainClass> allowedMainClasses = null,
        List<CardType> allowedCardTypes = null,
        List<Rarity> allowedCardRarities = null,
        int? allowedPrimaryManaCost = null,
        string cardNameKeyword = null,
        int? numberOfCards = null,
        bool allowDuplicates = false)
    {
        List<ScriptableCard> allCards = GetCardList(
            allowedMainClasses,
            allowedCardTypes,
            allowedCardRarities,
            allowedPrimaryManaCost,
            cardNameKeyword);

        if (allCards.Count == 0 || !numberOfCards.HasValue || numberOfCards.Value <= 0)
            return allCards;

        List<ScriptableCard> result = new List<ScriptableCard>();

        if (allowDuplicates)
        {
            for (int i = 0; i < numberOfCards.Value; i++)
            {
                ScriptableCard randomCard = allCards[UnityEngine.Random.Range(0, allCards.Count)];
                result.Add(randomCard);
            }
        }
        else
        {
            result = allCards.OrderBy(_ => UnityEngine.Random.value)
                             .Take(numberOfCards.Value)
                             .ToList();
        }

        return result;
    }


    public void OpenCardListChoice(List<ScriptableCard> scriptableCards, bool showCloseButton = true)
    {
        //display screen
        UIManager.Instance.ChooseGroupUI.SetActive(true);
        UIManager.Instance.ChooseGroupUI.transform.Find("TITLE").GetComponent<TMP_Text>().text = "CHOOSE A CARD!";

        if (showCloseButton)
        {
            UIManager.Instance.ChooseGroupUI.transform.Find("btn_Skip").gameObject.SetActive(true);
        }
        else
        {
            UIManager.Instance.ChooseGroupUI.transform.Find("btn_Skip").gameObject.SetActive(false);
        }

        //change the mode
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.CHOICE;

        //where to add card
        SystemManager.Instance.addCardTo = SystemManager.AddCardTo.Hand;

        GameObject parent = UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject;

        SystemManager.Instance.DestroyAllChildren(parent);

        foreach (ScriptableCard scriptableCard in scriptableCards)
        {
            CardScriptData cardScriptData = new CardScriptData();
            cardScriptData.scriptableCard = scriptableCard;

            GameObject card = DeckManager.Instance.InitializeCardPrefab(cardScriptData, parent, false, true);
        }
    }




    //    public Color32 GetClassColor(ScriptableCard scriptableCard)
    //    {

    //        Color32 color32 = whiteColor;

    //        if (scriptableCard.mainClass == CharacterManager.MainClass.Knight)
    //        {
    //            color32 = redColor;
    //        }
    //        else if (scriptableCard.mainClass == CharacterManager.MainClass.Rogue)
    //        {
    //color32 = 
    //        }

    //        Color colorFromHex;
    //        ColorUtility.TryParseHtmlString("#FFEC19", out colorFromHex);

    //        return color32;

    //    }

    public List<CardScriptData> GetAllCardsFromLibrary()
    {

        List<CardScriptData> cardScriptDataList = new List<CardScriptData>();
        //add all cards 
        foreach (CardPoolList cardPoolList in CardListManager.Instance.cardPoolLists)
        {
            foreach (ScriptableCard scriptableCard in cardPoolList.scriptableCards)
            {
                CardScriptData cardScriptData = new CardScriptData();
                cardScriptData.scriptableCard = scriptableCard;

                cardScriptDataList.Add(cardScriptData);
            }
        }

        return cardScriptDataList;

    }

    public List<string> GetAllClassCardsFromList(List<CardScriptData> cardScriptList)
    {
        var uniqueClasses = cardScriptList
            .Select(card => card.scriptableCard.mainClass.ToString())  // get all mainClass strings
            .Distinct()  // keep only unique ones
            .ToList();   // convert to List<string>

        return uniqueClasses;
    }

    public List<MainClass> ConvertStringListToMainClassList(List<string> stringList)
    {
        return stringList
            .Select(s => Enum.Parse<MainClass>(s)) // Convert each string to MainClass
            .ToList();
    }

    public bool CardExistsInList(List<CardScriptData> list, CardScriptData cardToCheck)
    {
        return list.Any(c => c.scriptableCard != null &&
                             c.scriptableCard.cardName == cardToCheck.scriptableCard.cardName);
    }

    public CardScriptData FindCardByName(List<CardScriptData> list, CardScriptData cardToCheck)
    {
        return list.FirstOrDefault(c => c.scriptableCard != null &&
                                        c.scriptableCard.cardName == cardToCheck.scriptableCard.cardName);
    }

}



