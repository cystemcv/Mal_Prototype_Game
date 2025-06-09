using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject cardPrefab;


    //classes card pool
    [FoldoutGroup("CARD POOLS")]
    public List<CardPoolList> cardPoolLists;

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
        int? allowedPrimaryManaCost = null)
    {
        return CardListManager.Instance.cardPoolLists
            .Where(pool =>
                allowedMainClasses == null || allowedMainClasses.Count == 0 ||
                allowedMainClasses.Contains(pool.mainClass))
            .SelectMany(pool => pool.scriptableCards)
            .Where(card =>
                (allowedCardTypes == null || allowedCardTypes.Count == 0 || allowedCardTypes.Contains(card.cardType)) &&
                (allowedCardRarities == null || allowedCardRarities.Count == 0 || allowedCardRarities.Contains(card.cardRarity)) &&
                (!allowedPrimaryManaCost.HasValue || card.primaryManaCost == allowedPrimaryManaCost.Value))
            .ToList();
    }


    public List<ScriptableCard> ChooseCards(
        List<MainClass> allowedMainClasses = null,
        List<CardType> allowedCardTypes = null,
        List<Rarity> allowedCardRarities = null,
        int? allowedPrimaryManaCost = null,
        int? numberOfCards = null,
        bool allowDuplicates = false)
    {
        List<ScriptableCard> allCards = GetCardList(
            allowedMainClasses,
            allowedCardTypes,
            allowedCardRarities,
            allowedPrimaryManaCost);

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
            // Shuffle and take up to numberOfCards without duplicates
            result = allCards.OrderBy(_ => UnityEngine.Random.value)
                             .Take(numberOfCards.Value)
                             .ToList();
        }

        return result;
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


}
