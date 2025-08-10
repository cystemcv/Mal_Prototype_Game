using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using static ScriptableCard;
using Michsky.MUIP;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    //combat deck
    public List<CardScriptData> combatDeck;

    //Discarded Cards
    public List<CardScriptData> discardedPile;

    //Banished Cards (Out of Play)
    public List<CardScriptData> banishedPile;

    //Hand Cards 
    public List<CardScriptData> handCards;


    public CardScript savedPlayedCard;
    public LTDescr savedtweenCardPlayed;

    [Header("FAN SHAPE")]
    //public List<GameObject> cards;
    public Transform handCenter; // The center point of your hand
    public float fanAngle = 0f; // The angle between each card in the fan
                                // Define spacing between cards before and after the index
    public float spacing = 150f;
    public float extraSpacingAfterIndex = 155f;

    public float drawCardWaitTime;
    public float playCardWaitTime;

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

    private void Start()
    {
        //-1 is no card hovering
        // ArrangeCardsHover(-1);

        //DeckManager.Instance.BuildStartingDeck();
    }



    public void BuildStartingDeck()
    {

        // Clear the deck before building a new one
        StaticData.staticMainDeck.Clear();

        //loop for each starting card list and add it to our deck
        foreach (ScriptableCard scriptableCard in StaticData.staticCharacter.startingCards)
        {

            //add cards to the deck
            CardScriptData cardScriptData = new CardScriptData();
            cardScriptData.scriptableCard = scriptableCard;
            StaticData.staticMainDeck.Add(cardScriptData);

        }

        // Shuffle the deck
        ShuffleDeck(StaticData.staticMainDeck);

    }


    public void ShuffleDeck(List<CardScriptData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardScriptData temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void DrawCardFromDeck()
    {

        //do not draw if both discard and deck pile is at 0

        //try
        //{

        if (combatDeck.Count != 0 || discardedPile.Count != 0)
        {

            //check if there are cards on deck/ if not put the discard pile back to deck (loop)
            FillUpDeckFromDiscardPile();

            //put the top 1 card from deck on hand
            CardScriptData cardScriptData = combatDeck[0];

            //if it passes the max hand limit the discard it, otherwise add it to the hand
            if (handCards.Count >= HandManager.Instance.maxHandCardsLimit)
            {

                //discard it
                discardedPile.Add(cardScriptData);

                HandFullSpawnCardFunction(cardScriptData);
            }
            else
            {

                //add it to the hand
                handCards.Add(cardScriptData);

                //instantiate the card
                InitializeCardPrefab(cardScriptData, UI_Combat.Instance.HAND, true, false);

                cardScriptData.scriptableCard.OnDrawCard(cardScriptData);

                //rearrange hand
                HandManager.Instance.SetHandCards();


            }

            //remove the top 1 card from deck
            combatDeck.RemoveAt(0);


        }
        //}
        //catch(Exception ex)
        //{
        //    Debug.LogError("Draw Card Error : " + ex.Message);
        //}
    }


    public void GetCardFromCombatDeckToHand(int index)
    {

        if (combatDeck.Count != 0)
        {

            CardScriptData cardScriptData = combatDeck[index];

            //Debug.Log("inside : " + cardScript.changedMana);

            //add it to the hand
            handCards.Add(cardScriptData);

            //instantiate the card
            InitializeCardPrefab(cardScriptData, UI_Combat.Instance.HAND, true, false);

            //rearrange hand
            HandManager.Instance.SetHandCards();

            //remove the top 1 card from deck
            combatDeck.RemoveAt(index);
        }
    }


    public void DiscardCardFromHand(CardScriptData cardScriptData)
    {
        //add it to the discard pile
        discardedPile.Add(cardScriptData);

        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScriptData.cardID);
        handCards.RemoveAt(index);

        //destroy the prefab
        DestroyCardPrefab(cardScriptData, SystemManager.CardThrow.DISCARD);



    }

    public void RemovePlayedCardFromHand(CardScriptData cardScriptData)
    {


        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScriptData.cardID);
        handCards.RemoveAt(index);


        index = -1;
        GameObject cardToDelete = null;

        //find the card object
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            //get the cardscript so we can compare the scriptable objects
            CardScriptData cardScriptDataFromGameObject = cardPrefab.GetComponent<CardScript>().cardScriptData;

            //if they are equal then we destroy it
            if (cardScriptDataFromGameObject.cardID == cardScriptData.cardID)
            {
                //find the index of the card we are discarding
                index = HandManager.Instance.cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardScriptData.cardID == cardScriptData.cardID);

                cardToDelete = cardPrefab;

                break;

            }
        }

        //remove it from the gameobject list
        HandManager.Instance.cardsInHandList.RemoveAt(index);

        ////parent it
        //cardPrefab.gameObject.transform.SetParent(UI_Combat.Instance.CARDPLAYED.transform);

        ////deactivate events
        //cardPrefab.gameObject.GetComponent<CardEvents>().enabled = false;

        //move the card
        savedtweenCardPlayed = LeanTween.move(cardToDelete.gameObject, UI_Combat.Instance.CARDPLAYED.transform, 0.2f);

        //save the cardscript
        //savedPlayedCardScript = cardScript;
    }

    public void BanishCardFromHand(CardScriptData cardScriptData)
    {
        //add it to the discard pile
        banishedPile.Add(cardScriptData);

        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScriptData.cardID);
        handCards.RemoveAt(index);

        //destroy the prefab
        DestroyCardPrefab(cardScriptData, SystemManager.CardThrow.BANISH);



    }

    public void DiscardWholeHand()
    {
        //foreach(cardScriptData handCard in handCards)
        //{
        //    DiscardCardFromHand(handCard);
        //}

        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            if (handCards[i].scriptableCard.discardCardAtEndTurn)
            {
                DiscardCardFromHand(handCards[i]);
            }

        }
    }

    public void DiscardCardFromHandRandom()
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = UnityEngine.Random.Range(0, handCards.Count);

        //call the discard function
        DiscardCardFromHand(handCards[randomIndex]);




    }

    public void PlayerPlayedCard(CardScriptData cardScriptData)
    {
        //save the cardScript temp
        CardScriptData tempCardScriptData = new CardScriptData();
        tempCardScriptData = cardScriptData;



        //remove from hand and add it to the played card
        //RemovePlayedCardFromHand(tempCardScript);



        //activate all card abilities
        PlayerPlayCardActivate(tempCardScriptData);

    }


    //public void PlayCard(CardScriptData cardScriptData)
    //{
    //    //check if the card has any abilities to be played
    //    if (cardScriptData.scriptableCard.cardAbilityClass.Count == 0)
    //    {
    //        return;
    //    }

    //    //save the cardScript temp
    //    CardScript tempCardScript = new CardScript();
    //    tempCardScript = cardScript;

    //    ////check the card conditions
    //    //bool condition = false;
    //    //condition = CheckCardOnlyConditions(tempCardScript);

    //    //if (condition)
    //    //{
    //    //    //show notification
    //    //    UI_Combat.Instance.OnNotification("CARD CONDITIONS NOT MET!", 1);
    //    //    return;
    //    //}


    //    //decrease available mana
    //    Combat.Instance.ManaAvailable -= cardScript.primaryManaCost;

    //    //remove from hand and add it to the played card
    //    RemovePlayedCardFromHand(tempCardScript);

    //    //activate all card abilities
    //    PlayCardOnlyAbilities(tempCardScript);


    //}

    //public bool CheckCardOnlyConditions(CardScriptData cardScriptData)
    //{
    //    bool condition = false;
    //    //get the player
    //    GameObject entity = GameObject.FindGameObjectWithTag("Player");

    //    if (entity == null)
    //    {
    //        return true;
    //    }

    //    foreach (CardConditionClass cardConditionClass in cardScript.scriptableCard.cardConditionClass)
    //    {

    //        if (cardConditionClass == null)
    //        {
    //            condition = false;
    //            break;
    //        }

    //        // Wait for 2 seconds
    //        if (cardConditionClass.ScriptableCardCondition.OnPlayCard(cardScriptData, cardConditionClass, entity, SystemManager.ControlBy.PLAYER))
    //        {
    //            condition = true;
    //            break;
    //        }


    //    }

    //    //variables needed for this activasion
    //    StaticData.artifact_CardScript = cardScript;
    //    ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayCard);

    //    return condition;
    //}


    public void PlayerPlayCardActivate(CardScriptData cardScriptData)
    {

        StartCoroutine(PlayerPlayCardActivateCoroutine(cardScriptData));

    }

    IEnumerator PlayerPlayCardActivateCoroutine(CardScriptData cardScriptData)
    {
        //get the player
        GameObject entity = GameObject.FindGameObjectWithTag("Player");
        cardScriptData.scriptableCard.OnPlayCard(cardScriptData, entity);

        yield return StartCoroutine(BuffSystemManager.Instance.ActivateBuffsDebuffs_OnPlayCard(cardScriptData, entity, null));

        CardPlayedStaticVariables(cardScriptData);

        if (savedPlayedCard != null)
        {
            //remove all the tooltips
            savedPlayedCard.GetComponent<TooltipContent>().StartExit();

            //destroy the prefab
            if (savedPlayedCard.cardScriptData.scriptableCard.cardType == SystemManager.CardType.Focus)
            {
                DestroyPlayedCard(SystemManager.CardThrow.BANISH);
            }
            else
            {
                DestroyPlayedCard(SystemManager.CardThrow.DISCARD);
            }

        }

        StaticData.artifact_CardScript = cardScriptData;
        yield return StartCoroutine(ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayCard, cardScriptData));

        yield return null;
    }


    public void CardPlayedStaticVariables(CardScriptData cardScriptData)
    {

        StaticData.IncrementStat(StaticData.combatStats, "Cards_Played", 1);
        StaticData.IncrementStat(StaticData.combatStats, "Total_Cards_Played", 1);

        if (cardScriptData.scriptableCard.primaryManaCost == 0)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Zero_Mana_Cost_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Zero_Mana_Cost_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.primaryManaCost == 1)
        {
            StaticData.IncrementStat(StaticData.combatStats, "One_Mana_Cost_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_One_Mana_Cost_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.primaryManaCost == 2)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Two_Mana_Cost_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Two_Mana_Cost_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.primaryManaCost == 3)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Three_Mana_Cost_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Three_Mana_Cost_Cards_Played", 1);
        }
        else
        {
            StaticData.IncrementStat(StaticData.combatStats, "Four_And_Above_Mana_Cost_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Four_And_Above_Mana_Cost_Cards_Played", 1);
        }

        if (cardScriptData.scriptableCard.cardType == SystemManager.CardType.Attack)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Attack_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Attack_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.cardType == SystemManager.CardType.Skill)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Skill_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Skill_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.cardType == SystemManager.CardType.Focus)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Focus_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Focus_Cards_Played", 1);
        }
        else if (cardScriptData.scriptableCard.cardType == SystemManager.CardType.Magic)
        {
            StaticData.IncrementStat(StaticData.combatStats, "Magic_Cards_Played", 1);
            StaticData.IncrementStat(StaticData.combatStats, "Total_Magic_Cards_Played", 1);
        }

        Debug.Log(StaticData.FormatStatsForText(StaticData.combatStats));

    }

    //public void PlayCardOnlyAbilities(CardScriptData cardScriptData)
    //{

    //    StartCoroutine(PlayCardCoroutine(cardScriptData));

    //}

    //IEnumerator PlayCardCoroutine(CardScriptData cardScriptData)
    //{

    //    SystemManager.Instance.thereIsActivatedCard = true;

    //    //get the player
    //    GameObject entity = GameObject.FindGameObjectWithTag("Player");

    //    if (entity == null)
    //    {
    //        yield return null;
    //    }

    //    foreach (CardAbilityClass cardAbilityClass in cardScript.scriptableCard.cardAbilityClass)
    //    {

    //        if (cardAbilityClass == null)
    //        {
    //            continue;
    //        }

    //        // Wait for 2 seconds
    //        cardAbilityClass.scriptableCardAbility.OnPlayCard(cardScriptData, cardAbilityClass, entity, SystemManager.ControlBy.PLAYER);


    //        //check to reset mana to the original cost if neeeded
    //        if (cardScriptData.resetManaCost)
    //        {
    //            cardScript.primaryManaCost = cardScript.scriptableCard.primaryManaCost;
    //            cardScript.resetManaCost = false;
    //            cardScript.changedMana = false;
    //        }



    //        yield return new WaitForSeconds(cardAbilityClass.waitForAbility);
    //        //yield return new WaitForSeconds(10f);
    //    }

    //    //go back to idle animation
    //    if (entity != null)
    //    {
    //        Animator animator = entity.transform.Find("model").GetComponent<Animator>();

    //        if (animator != null)
    //        {

    //            animator.SetTrigger("Idle");
    //        }

    //    }



    //    //card ended
    //    SystemManager.Instance.thereIsActivatedCard = false;

    //    //if not already discarded by abilities then discard
    //    if (savedPlayedCardScript != null)
    //    {
    //        //destroy the prefab
    //        DestroyPlayedCard(SystemManager.CardThrow.DISCARD);
    //        //DiscardCardFromHand(cardScriptData);
    //    }

    //}

    public void PlayCardFromCombatDeck(int index)
    {

        if (combatDeck.Count == 0 && discardedPile.Count == 0)
        {
            return;
        }

        //check deck
        if (combatDeck.Count <= 0)
        {

            //check if there are cards on deck/ if not put the discard pile back to deck (loop)
            FillUpDeckFromDiscardPile();
        }


        CardScriptData cardScriptData = combatDeck[index];
        HandFullSpawnCardFunction(cardScriptData);

        //save card
        savedPlayedCard.cardScriptData = cardScriptData;

        //get card targets 
        List<string> tagsInCard = new List<string>();

        foreach (SystemManager.EntityTag entityTag in cardScriptData.scriptableCard.targetEntityTagList)
        {
            tagsInCard.Add(entityTag.ToString());
        }

        List<GameObject> allEntities = SystemManager.Instance.FindGameObjectsWithTags(tagsInCard);

        //get random target if it has a target
        if (allEntities.Count > 0)
        {
            int randomEntityIndex = UnityEngine.Random.Range(0, allEntities.Count);

            CombatCardHandler.Instance.targetClicked = allEntities[randomEntityIndex];
        }


        //get the player
        GameObject entity = GameObject.FindGameObjectWithTag("Player");
        cardScriptData.scriptableCard.OnPlayCard(cardScriptData, entity);

        if (savedPlayedCard.cardScriptData.scriptableCard != null)
        {
            //destroy the prefab
            if (savedPlayedCard.cardScriptData.scriptableCard.cardType == SystemManager.CardType.Focus)
            {
                combatDeck.RemoveAt(index);
                banishedPile.Add(savedPlayedCard.cardScriptData);

            }
            else
            {
                combatDeck.RemoveAt(index);
                discardedPile.Add(savedPlayedCard.cardScriptData);
            }

        }
    }


    public void PlayCardFromHandRandom(bool customManaCostEnable = false, int customManaCost = 0)
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = UnityEngine.Random.Range(0, handCards.Count);

        ////test the desc
        //Debug.Log(GenerateCardAbilityDescription(handCards[randomIndex]));

        ////play the card
        //PlayCard(handCards[randomIndex]);

        CardScriptData cardScriptData = handCards[randomIndex];

        HandFullSpawnCardFunction(cardScriptData);

        //save card
        savedPlayedCard.cardScriptData = cardScriptData;



        //random target
        List<GameObject> allEntities = SystemManager.Instance.FindGameObjectsWithTags(SystemManager.Instance.GetAllTagsList());

        //get random target
        int randomEntityIndex = UnityEngine.Random.Range(0, allEntities.Count);

        CombatCardHandler.Instance.targetClicked = allEntities[randomEntityIndex];

        //decrease available mana
        if (customManaCostEnable)
        {
            Combat.Instance.ManaAvailable -= customManaCost;
        }
        else
        {
            Combat.Instance.ManaAvailable -= cardScriptData.scriptableCard.primaryManaCost;
        }

        if (Combat.Instance.ManaAvailable <= 0)
        {
            Combat.Instance.ManaAvailable = 0;
        }

        //call the discard function
        DiscardCardFromHand(cardScriptData);
        PlayerPlayCardActivate(cardScriptData);

    }

    public IEnumerator DrawMultipleCards(int numberOfCards, float waitBeforeDrawing)
    {

        yield return new WaitForSeconds(waitBeforeDrawing);

        //draw cards
        yield return StartCoroutine(DrawMultipleCardsCoroutine(numberOfCards));
    }

    IEnumerator DrawMultipleCardsCoroutine(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            //if its the first card then draw without delay
            if (i == 0)
            {

                DrawCardFromDeck();
                i++;
            }

            // Wait for 2 seconds
            yield return new WaitForSeconds(drawCardWaitTime);
            DrawCardFromDeck();


        }

    }


    public void FillUpDeckFromDiscardPile()
    {
        if (combatDeck.Count == 0)
        {

            //randomize list

            //add to deck and remove from discard pile
            foreach (CardScriptData cardScriptData in discardedPile)
            {

                combatDeck.Add(cardScriptData);
            }

            //clear the discarded pile
            discardedPile.Clear();
        }

        ShuffleDeck(combatDeck);
    }


    public void AddCardOnDeck(ScriptableCard scriptableCard, int character)
    {

        //add a card ti deck
        //GameObject cardPrefab = CardListManager.Instance.cardPrefab;
        //cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;

        //CardScript cardScript = new CardScript();

        CardScriptData cardScriptData = new CardScriptData();
        cardScriptData.scriptableCard = scriptableCard;
        StaticData.staticMainDeck.Add(cardScriptData);


    }

    public void AddCardOnCombatDeck(CardScriptData cardScriptData)
    {
        DeckManager.Instance.combatDeck.Add(cardScriptData);
    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {

        // deck.Remove(card);

    }

    public GameObject InitializeCardPrefab(CardScriptData cardScriptData, GameObject parent, bool addToHand, bool normalUI)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, parent.transform.position, Quaternion.identity);
        //get the scriptable object
        ScriptableCard scriptableCard = cardScriptData.scriptableCard;

        //disable scripts not needed
        cardPrefab.GetComponent<CardScript>().enabled = true;
        cardPrefab.GetComponent<CardEvents>().enabled = true;

        //enable scripts needed
        cardPrefab.GetComponent<CardListCardEvents>().enabled = false;
        cardPrefab.GetComponent<Button>().enabled = false;
        cardPrefab.GetComponent<CustomButton>().enabled = false;

        //add the scriptable card object to the prefab class to reference
        CardScriptData cardScriptDataOnCard = new CardScriptData();

        cardScriptDataOnCard.scriptableCard = scriptableCard;
        cardScriptDataOnCard.cardID = cardScriptData.cardID;
        cardScriptDataOnCard.changedMana = cardScriptData.changedMana;
        cardScriptDataOnCard.resetManaCost = cardScriptData.resetManaCost;
        cardScriptDataOnCard.scalingLevelValue = cardScriptData.scalingLevelValue;

        cardScriptDataOnCard.cardQueue = cardPrefab.transform.Find("Panel").Find("UtilityFront").Find("Queue").gameObject;

        cardPrefab.GetComponent<CardScript>().cardScriptData = cardScriptDataOnCard;

        if (scriptableCard.toggle1)
        {

            //enable custom filtering
            GameObject customTargeting = cardPrefab.transform.Find("Panel").Find("UtilityFront").Find("CustomTargeting").gameObject;
            customTargeting.SetActive(true);

            GameObject panel = customTargeting.transform.Find("Panel").gameObject;
            panel.transform.GetChild(0).GetComponent<Image>().sprite = UI_Combat.Instance.fillBox;

            if(scriptableCard.toggle2)
            {
                panel.transform.GetChild(1).GetComponent<Image>().sprite = UI_Combat.Instance.fillBox;
            }
            else
            {
                panel.transform.GetChild(1).GetComponent<Image>().sprite = UI_Combat.Instance.emptyBox;
            }

            if (scriptableCard.toggle3)
            {
                panel.transform.GetChild(2).GetComponent<Image>().sprite = UI_Combat.Instance.fillBox;
            }
            else
            {
                panel.transform.GetChild(2).GetComponent<Image>().sprite = UI_Combat.Instance.emptyBox;
            }

            if (scriptableCard.toggle4)
            {
                panel.transform.GetChild(3).GetComponent<Image>().sprite = UI_Combat.Instance.fillBox;
            }
            else
            {
                panel.transform.GetChild(3).GetComponent<Image>().sprite = UI_Combat.Instance.emptyBox;
            }

            if (scriptableCard.toggle5)
            {
                panel.transform.GetChild(4).GetComponent<Image>().sprite = UI_Combat.Instance.fillBox;
            }
            else
            {
                panel.transform.GetChild(4).GetComponent<Image>().sprite = UI_Combat.Instance.emptyBox;
            }

        }


        cardPrefab.GetComponent<CardScript>().cardScriptData.cardQueue.SetActive(false);
        //Debug.Log("cardPrefab.GetComponent<CardScript>().changedMana : " + cardPrefab.GetComponent<CardScript>().changedMana);

        if (cardScriptData.changedMana == false)
        {
            cardPrefab.GetComponent<CardScript>().cardScriptData.primaryManaCost = scriptableCard.primaryManaCost;
        }


        //set it as a child of the parent
        cardPrefab.transform.SetParent(parent.transform);


        if (normalUI)
        {
            cardPrefab.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        else
        {
            //make the local scale 1,1,1
            cardPrefab.transform.localScale = new Vector3(1, 1, 1);

            //add sorting 
            cardPrefab.GetComponent<CardEvents>().sortOrder = 1200;
            cardPrefab.GetComponent<Canvas>().sortingOrder = 1200;
        }

        UpdateCardUI(cardPrefab);

        //add it to the hand list
        if (addToHand)
        {
            HandManager.Instance.cardsInHandList.Add(cardPrefab);
        }

        //check mana
        if (Combat.Instance != null)
        {
            Combat.Instance.UpdateCardAfterManaChange(cardPrefab);
        }

        return cardPrefab;

    }

    public void UpdateCardUI(GameObject cardPrefab)
    {
       
        CardScriptData cardScriptData = cardPrefab.GetComponent<CardScript>().cardScriptData;
        ScriptableCard scriptableCard = cardScriptData.scriptableCard;
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        if (scriptableCard.cardRarity == SystemManager.Rarity.Common)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.commonBg;
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Rare)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.rareBg;
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Epic)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.epicBg;
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Legendary)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.legendaryBg;
        }
        else if (scriptableCard.cardRarity == SystemManager.Rarity.Curse)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.curseBg;
        }

        //for example
        cardChild.transform.Find("Info").Find("TitleText").GetComponent<TMP_Text>().text = scriptableCard.cardName;
        cardChild.transform.Find("Info").Find("TitleText").GetComponent<TMP_Text>().text += ShowCardLevel(cardPrefab.GetComponent<CardScript>().cardScriptData);

        cardChild.transform.Find("Info").Find("CardImage").GetComponent<Image>().sprite = scriptableCard.cardArt;
        cardChild.transform.Find("Info").Find("TypeText").GetComponent<TMP_Text>().text = scriptableCard.cardType.ToString();

    
        //mana cost
        cardChild.transform.Find("Info").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>().text = cardScriptData.primaryManaCost.ToString();

        if (scriptableCard.cardRarity == SystemManager.Rarity.Curse)
        {
            cardChild.transform.Find("Info").Find("ManaImage").gameObject.SetActive(false);
        }

        cardChild.transform.Find("Info").Find("DescriptionText").GetComponent<TMP_Text>().text = scriptableCard.OnCardDescription(cardScriptData, character);


        if (scriptableCard.scriptableKeywords.Count > 0)
        {
            cardChild.transform.Find("Info").Find("DescriptionText").GetComponent<TMP_Text>().text += "\n";
        }

        foreach (ScriptableKeywords scriptableKeyword in scriptableCard.scriptableKeywords)
        {
            cardChild.transform.Find("Info").Find("DescriptionText").GetComponent<TMP_Text>().text += "<color=yellow>" + scriptableKeyword.keywordName + "</color> ";
        }



    }

    public void UpdateCardArtifactUI(ClassItemData classItemData, GameObject cardPrefab)
    {

        ScriptableItem scriptableItem = classItemData.scriptableItem;
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        if (scriptableItem.itemRarity == SystemManager.Rarity.Common)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.commonBg;
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Rare)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.rareBg;
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Epic)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.epicBg;
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Legendary)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.legendaryBg;
        }
        else if (scriptableItem.itemRarity == SystemManager.Rarity.Curse)
        {
            cardChild.transform.Find("Info").Find("MainBgFront").GetComponent<Image>().sprite = CardListManager.Instance.curseBg;
        }

        //for example
        cardChild.transform.Find("Info").Find("TitleText").GetComponent<TMP_Text>().text = scriptableItem.itemName;

        cardChild.transform.Find("Info").Find("CardImageIcon").gameObject.SetActive(true);
        cardChild.transform.Find("Info").Find("CardImageIcon").GetComponent<Image>().sprite = scriptableItem.Icon;
        cardChild.transform.Find("Info").Find("TypeText").GetComponent<TMP_Text>().text = scriptableItem.itemCategory.ToString();

        cardChild.transform.Find("Info").Find("ManaImage").gameObject.SetActive(false);
 
        cardChild.transform.Find("Info").Find("DescriptionText").GetComponent<TMP_Text>().text = scriptableItem.itemDescription;



    }


    public void UpdateCardDescription(GameObject cardPrefab)
    {

        CardScriptData cardScriptData = cardPrefab.GetComponent<CardScript>().cardScriptData;
        ScriptableCard scriptableCard = cardScriptData.scriptableCard;
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = scriptableCard.OnCardDescription(cardScriptData, character); ;

    }

    public GameObject InitializeCardArtifactPrefab(ClassItemData classItemData, GameObject parent, bool addToHand, bool normalUI)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, parent.transform.position, Quaternion.identity);
        //get the scriptable object
        ScriptableItem scriptableItem = classItemData.scriptableItem;

        //disable scripts not needed
        cardPrefab.GetComponent<CardScript>().enabled = false;
        cardPrefab.GetComponent<CardEvents>().enabled = false;

        //enable scripts needed
        cardPrefab.GetComponent<CardListCardEvents>().enabled = false;
        cardPrefab.GetComponent<Button>().enabled = false;
        cardPrefab.GetComponent<CustomButton>().enabled = false;

        ItemChoiceClass itemChoiceClass = cardPrefab.AddComponent<ItemChoiceClass>();
        itemChoiceClass.classItem = classItemData;

        //set it as a child of the parent
        cardPrefab.transform.SetParent(parent.transform);


        if (normalUI)
        {
            cardPrefab.transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }
        else
        {
            //make the local scale 1,1,1
            cardPrefab.transform.localScale = new Vector3(1, 1, 1);

            //add sorting 
            cardPrefab.GetComponent<Canvas>().sortingOrder = 1200;
        }

        UpdateCardArtifactUI(classItemData, cardPrefab);

        //add it to the hand list
        if (addToHand)
        {
            HandManager.Instance.cardsInHandList.Add(cardPrefab);
        }

        ////check mana
        //if (Combat.Instance != null)
        //{
        //    Combat.Instance.UpdateCardAfterManaChange(cardPrefab);
        //}

        return cardPrefab;

    }

    public string GetBonusAttackAsDescription(int cardDmg, int calculatedDmg)
    {

        int result = calculatedDmg - cardDmg;

        if (result > 0)
        {
            return "<color=green>(+" + result + ")</color>";
        }
        else if (result < 0)
        {
            return "<color=red>(-" + Mathf.Abs(result) + ")</color>";
        }
        else
        {
            return "";
        }
    }

    public string GetCalculatedValueString(int originalValue, int calculatedValue)
    {

        int result = calculatedValue - originalValue;


        if (calculatedValue < 0)
        {
            calculatedValue = 0;
        }

        if (result > 0)
        {
            return "<color=green>" + calculatedValue + "</color>";
        }
        else if (result < 0)
        {
            return "<color=red>" + Mathf.Abs(calculatedValue) + "</color>";
        }
        else
        {
            return calculatedValue.ToString();
        }
    }

    //public Color32 AssignCardColor()
    //{
    //    Color32 colorToChange;

    //    if ()
    //    {

    //    }
    //}

    public void HandFullSpawnCardFunction(CardScriptData cardScriptData)
    {
        //create gameobject on scene and spawn it on the discard spawner

        GameObject cardPrefab = DeckManager.Instance.InitializeCardPrefab(cardScriptData, UI_Combat.Instance.handFullSpawnCard, false, false);
        cardPrefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        //deactivate events
        cardPrefab.GetComponent<CardEvents>().enabled = false;

        StartCoroutine(DestroyHandExceedCard(cardPrefab));


    }

    IEnumerator DestroyHandExceedCard(GameObject cardPrefab)
    {

        // Wait for 2 seconds
        yield return new WaitForSeconds(1f);
        Destroy(cardPrefab);
        GameObject cardSoul = Instantiate(CombatCardHandler.Instance.discardEffect, UI_Combat.Instance.handFullSpawnCard.transform.position, Quaternion.identity);
        //and assign the target
        cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardInfo;


    }


    public void DestroyPlayedCard(SystemManager.CardThrow cardThrow)
    {
        //instantiate the effect
        GameObject cardSoul;

        if (cardThrow == SystemManager.CardThrow.DISCARD)
        {
            //add it to the discard pile
            discardedPile.Add(savedPlayedCard.cardScriptData);

            cardSoul = Instantiate(CombatCardHandler.Instance.discardEffect, savedPlayedCard.gameObject.transform.position, Quaternion.identity);
            //and assign the target
            cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardInfo;
        }
        else if (cardThrow == SystemManager.CardThrow.BANISH)
        {
            //add it to the banished pile
            banishedPile.Add(savedPlayedCard.cardScriptData);

            cardSoul = Instantiate(CombatCardHandler.Instance.banishEffect, savedPlayedCard.gameObject.transform.position, Quaternion.identity);
            //and assign the target
            cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.banishedInfo;
        }

        //destroy it as we do not need it anymore
        Destroy(savedPlayedCard.gameObject);

        //rearrange hand
        HandManager.Instance.SetHandCards();
    }

    public string ShowCardLevel(CardScriptData cardScriptData)
    {
        string text = "";

        if (cardScriptData.scalingLevelValue != 0)
        {
            text = " Lv" + cardScriptData.scalingLevelValue;
        }

        return text;
    }

    public void DestroyCardPrefab(CardScriptData cardScriptData, SystemManager.CardThrow cardThrow)
    {

        int index = -1;
        GameObject cardToDelete = null;

        //find the card object
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            //get the cardscript so we can compare the scriptable objects
            CardScriptData cardScriptDataFromGameObject = cardPrefab.GetComponent<CardScript>().cardScriptData;

            //if they are equal then we destroy it
            if (cardScriptDataFromGameObject.cardID == cardScriptData.cardID)
            {
                //find the index of the card we are discarding
                index = HandManager.Instance.cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardScriptData.cardID == cardScriptData.cardID);

                cardToDelete = cardPrefab;

                break;

            }
        }

        if (index != -1)
        {

            //remove it from the gameobject list
            HandManager.Instance.cardsInHandList.RemoveAt(index);

            //instantiate the effect
            GameObject cardSoul;

            if (cardThrow == SystemManager.CardThrow.DISCARD)
            {
                cardSoul = Instantiate(CombatCardHandler.Instance.discardEffect, cardToDelete.transform.position, Quaternion.identity);
                //and assign the target
                cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardInfo;
            }
            else if (cardThrow == SystemManager.CardThrow.BANISH)
            {
                cardSoul = Instantiate(CombatCardHandler.Instance.banishEffect, cardToDelete.transform.position, Quaternion.identity);
                //and assign the target
                cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.banishedInfo;
            }

            //destroy it as we do not need it anymore
            Destroy(cardToDelete);

            //rearrange hand
            HandManager.Instance.SetHandCards();
        }


    }

    public void AddScriptableCardToHand(ScriptableCard scriptableCard)
    {
        SystemManager.Instance.addCardTo = SystemManager.AddCardTo.Hand;

        CardScriptData tempCardScriptData = new CardScriptData();
        tempCardScriptData.scriptableCard = scriptableCard;

        AddCardToList(tempCardScriptData);

    }

    public void AddCardToList(CardScriptData cardScriptData)
    {

        if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.Hand)
        {

            if (handCards.Count >= HandManager.Instance.maxHandCardsLimit)
            {
                return;
            }

            //add to deck then draw it
            combatDeck.Add(cardScriptData);

            //get the index then draw it
            int combatDeckIndex = combatDeck.FindIndex(card => card.cardID == cardScriptData.cardID);

            DeckManager.Instance.GetCardFromCombatDeckToHand(combatDeckIndex);

        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.discardPile)
        {
            discardedPile.Add(cardScriptData);
        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.combatDeck)
        {
            combatDeck.Add(cardScriptData);
        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.mainDeck)
        {
            StaticData.staticMainDeck.Add(cardScriptData);

        }

        //close the thing 
        UIManager.Instance.ChooseGroupUI.SetActive(false);

        //resume
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

        SystemManager.Instance.DestroyAllChildren(UIManager.Instance.ChooseGroupUI.transform.Find("ChooseContainer").gameObject);

    }

    public int GetIntValueFromList(int position, List<int> list)
    {



        int value = 0;

        if (list.Count > position && position >= 0)
        {
            value = list[position];
        }

        return value;

    }


}
