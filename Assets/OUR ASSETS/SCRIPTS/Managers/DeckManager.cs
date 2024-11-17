using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using static ScriptableCard;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    //combat deck
    public List<CardScript> combatDeck;

    //Discarded Cards
    public List<CardScript> discardedPile;

    //Banished Cards (Out of Play)
    public List<CardScript> banishedPile;

    //Hand Cards 
    public List<CardScript> handCards;


    public CardScript savedPlayedCardScript;
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
            CardScript cardScript = new CardScript();
            cardScript.scriptableCard = scriptableCard;
            StaticData.staticMainDeck.Add(cardScript);

        }

        // Shuffle the deck
        ShuffleDeck(StaticData.staticMainDeck);

    }


    public void ShuffleDeck(List<CardScript> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardScript temp = list[i];
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
            CardScript cardScript = combatDeck[0];

            //if it passes the max hand limit the discard it, otherwise add it to the hand
            if (handCards.Count >= HandManager.Instance.maxHandCardsLimit)
            {

                //discard it
                discardedPile.Add(cardScript);

                HandFullSpawnCardFunction(cardScript);
            }
            else
            {

                //add it to the hand
                handCards.Add(cardScript);

                //instantiate the card
                InitializeCardPrefab(cardScript, UI_Combat.Instance.HAND, true, false);

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

            CardScript cardScript = combatDeck[index];

            //Debug.Log("inside : " + cardScript.changedMana);

            //add it to the hand
            handCards.Add(cardScript);

            //instantiate the card
            InitializeCardPrefab(cardScript, UI_Combat.Instance.HAND, true, false);

            //rearrange hand
            HandManager.Instance.SetHandCards();

            //remove the top 1 card from deck
            combatDeck.RemoveAt(index);
        }
    }


    public void DiscardCardFromHand(CardScript cardScript)
    {
        //add it to the discard pile
        discardedPile.Add(cardScript);

        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScript.cardID);
        handCards.RemoveAt(index);

        //destroy the prefab
        DestroyCardPrefab(cardScript, SystemManager.CardThrow.DISCARD);



    }

    public void RemovePlayedCardFromHand(CardScript cardScript)
    {


        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScript.cardID);
        handCards.RemoveAt(index);


        index = -1;
        GameObject cardToDelete = null;

        //find the card object
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            //get the cardscript so we can compare the scriptable objects
            CardScript cardScriptFromGameObject = cardPrefab.GetComponent<CardScript>();

            //if they are equal then we destroy it
            if (cardScriptFromGameObject.cardID == cardScript.cardID)
            {
                //find the index of the card we are discarding
                index = HandManager.Instance.cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardID == cardScript.cardID);

                cardToDelete = cardPrefab;

                break;

            }
        }

        //remove it from the gameobject list
        HandManager.Instance.cardsInHandList.RemoveAt(index);

        //parent it
        cardScript.gameObject.transform.SetParent(UI_Combat.Instance.CARDPLAYED.transform);

        //deactivate events
        cardScript.gameObject.GetComponent<CardEvents>().enabled = false;

        //move the card
        savedtweenCardPlayed = LeanTween.move(cardScript.gameObject, UI_Combat.Instance.CARDPLAYED.transform, 0.2f);

        //save the cardscript
        savedPlayedCardScript = cardScript;
    }

    public void BanishCardFromHand(CardScript cardScript)
    {
        //add it to the discard pile
        banishedPile.Add(cardScript);

        //remove the card based on the index
        int index = handCards.FindIndex(item => item.cardID == cardScript.cardID);
        handCards.RemoveAt(index);

        //destroy the prefab
        DestroyCardPrefab(cardScript, SystemManager.CardThrow.BANISH);



    }

    public void DiscardWholeHand()
    {
        //foreach(CardScript handCard in handCards)
        //{
        //    DiscardCardFromHand(handCard);
        //}

        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            DiscardCardFromHand(handCards[i]);
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

    public void PlayCard(CardScript cardScript)
    {
        //check if the card has any abilities to be played
        if (cardScript.scriptableCard.cardAbilityClass.Count == 0)
        {
            return;
        }

        //save the cardScript temp
        CardScript tempCardScript = new CardScript();
        tempCardScript = cardScript;

        //check the card conditions
        bool condition = false;
        condition = CheckCardOnlyConditions(tempCardScript);

        if (condition)
        {
            //show notification
            UI_Combat.Instance.OnNotification("CARD CONDITIONS NOT MET!", 1);
            return;
        }


        //decrease available mana
        Combat.Instance.ManaAvailable -= cardScript.primaryManaCost;

        //remove from hand and add it to the played card
        RemovePlayedCardFromHand(tempCardScript);

        //activate all card abilities
        PlayCardOnlyAbilities(tempCardScript);


    }

    public bool CheckCardOnlyConditions(CardScript cardScript)
    {
        bool condition = false;
        //get the player
        GameObject entity = GameObject.FindGameObjectWithTag("Player");

        if (entity == null)
        {
            return true;
        }

        foreach (CardConditionClass cardConditionClass in cardScript.scriptableCard.cardConditionClass)
        {

            if (cardConditionClass == null)
            {
                condition = false;
                break;
            }

            // Wait for 2 seconds
            if (cardConditionClass.ScriptableCardCondition.OnPlayCard(cardScript, cardConditionClass, entity, SystemManager.ControlBy.PLAYER))
            {
                condition = true;
                break;
            }


        }

        //variables needed for this activasion
        StaticData.artifact_CardScript = cardScript;
        ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnPlayCard);

        return condition;
    }

    public void PlayCardOnlyAbilities(CardScript cardScript)
    {

        StartCoroutine(PlayCardCoroutine(cardScript));

    }

    IEnumerator PlayCardCoroutine(CardScript cardScript)
    {

        SystemManager.Instance.thereIsActivatedCard = true;

        //get the player
        GameObject entity = GameObject.FindGameObjectWithTag("Player");

        if (entity == null)
        {
            yield return null;
        }

        foreach (CardAbilityClass cardAbilityClass in cardScript.scriptableCard.cardAbilityClass)
        {

            if (cardAbilityClass == null)
            {
                continue;
            }

            // Wait for 2 seconds
            cardAbilityClass.scriptableCardAbility.OnPlayCard(cardScript, cardAbilityClass, entity, SystemManager.ControlBy.PLAYER);


            //check to reset mana to the original cost if neeeded
            if (cardScript.resetManaCost)
            {
                cardScript.primaryManaCost = cardScript.scriptableCard.primaryManaCost;
                cardScript.resetManaCost = false;
                cardScript.changedMana = false;
            }



            yield return new WaitForSeconds(cardAbilityClass.waitForAbility);
            //yield return new WaitForSeconds(10f);
        }

        //go back to idle animation
        if (entity != null)
        {
            Animator animator = entity.transform.Find("model").GetComponent<Animator>();

            if (animator != null)
            {

                animator.SetTrigger("Idle");
            }

        }



        //card ended
        SystemManager.Instance.thereIsActivatedCard = false;

        //if not already discarded by abilities then discard
        if (savedPlayedCardScript != null)
        {
            //destroy the prefab
            DestroyPlayedCard(SystemManager.CardThrow.DISCARD);
            //DiscardCardFromHand(cardScript);
        }

    }


    public void PlayCardFromHandRandom()
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

        //call the discard function
        DiscardCardFromHand(handCards[randomIndex]);


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
            foreach (CardScript cardScript in discardedPile)
            {

                combatDeck.Add(cardScript);
            }

            //clear the discarded pile
            discardedPile.Clear();
        }

        ShuffleDeck(combatDeck);
    }


    public void AddCardOnDeck(ScriptableCard scriptableCard, int character)
    {

        //add a card ti deck
        GameObject cardPrefab = CardListManager.Instance.cardPrefab;
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        StaticData.staticMainDeck.Add(null);

    }

    public void AddCardOnCombatDeck(CardScript cardScript)
    {

    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {

        // deck.Remove(card);

    }

    public void InitializeCardPrefab(CardScript cardScript, GameObject parent, bool addToHand, bool normalUI)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, UI_Combat.Instance.deckUIObject.transform.position, Quaternion.identity);
        //get the scriptable object
        ScriptableCard scriptableCard = cardScript.scriptableCard;

        //add the scriptable card object to the prefab class to reference
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        cardPrefab.GetComponent<CardScript>().cardID = cardScript.cardID;
        cardPrefab.GetComponent<CardScript>().changedMana = cardScript.changedMana;
        cardPrefab.GetComponent<CardScript>().resetManaCost = cardScript.resetManaCost;

        //Debug.Log("cardPrefab.GetComponent<CardScript>().changedMana : " + cardPrefab.GetComponent<CardScript>().changedMana);

        if (cardScript.changedMana == false)
        {
            cardPrefab.GetComponent<CardScript>().primaryManaCost = scriptableCard.primaryManaCost;
        }


        //set it as a child of the parent
        cardPrefab.transform.SetParent(parent.transform);



        //use the scriptable object to fill the art, text (title,desc,mana cost,etc)
        //for text USE TEXT MESH PRO

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
        Combat.Instance.UpdateCardAfterManaChange(cardPrefab);

    }

    public void UpdateCardUI(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = GameObject.FindGameObjectWithTag("Player");

        //for example
        cardChild.transform.Find("TitleBg").Find("TitleText").GetComponent<TMP_Text>().text = scriptableCard.cardName;
        cardChild.transform.Find("CardImage").GetComponent<Image>().sprite = scriptableCard.cardArt;
        cardChild.transform.Find("TypeBg").Find("TypeText").GetComponent<TMP_Text>().text = scriptableCard.cardType.ToString();

        //cardChild.transform.Find("MainBg").GetComponent<Image>().color = CardListManager.Instance.GetClassColor(scriptableCard.mainClass);

        //mana cost
        cardChild.transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>().text = cardScript.primaryManaCost.ToString();
        //cardChild.transform.Find("ManaBg").Find("SecondaryManaImage").Find("SecondaryManaText").GetComponent<TMP_Text>().text = scriptableCard.primaryManaCost.ToString();

        //description is based on abilities
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "";
        foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += cardAbilityClass.scriptableCardAbility.AbilityDescription(cardScript, cardAbilityClass, character) + "\n";
        }

        //conditions
        foreach (CardConditionClass cardConditionClass in scriptableCard.cardConditionClass)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += cardConditionClass.ScriptableCardCondition.ConditionDescription(cardScript, cardConditionClass, character) + "\n";
        }

        //activation should not be visible
        cardChild.transform.Find("Activation").GetComponent<Image>().color = SystemManager.Instance.GetColorFromHex(SystemManager.Instance.colorTransparent);
    }


    public void UpdateCardDescription(GameObject cardPrefab)
    {

        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "";
        foreach (CardAbilityClass cardAbilityClass in scriptableCard.cardAbilityClass)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += cardAbilityClass.scriptableCardAbility.AbilityDescription(cardScript, cardAbilityClass, character) + "\n";
        }

        //conditions
        foreach (CardConditionClass cardConditionClass in scriptableCard.cardConditionClass)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += cardConditionClass.ScriptableCardCondition.ConditionDescription(cardScript, cardConditionClass, character) + "\n";
        }
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

    //public Color32 AssignCardColor()
    //{
    //    Color32 colorToChange;

    //    if ()
    //    {

    //    }
    //}

    public void HandFullSpawnCardFunction(CardScript cardScript)
    {
        //create gameobject on scene and spawn it on the discard spawner
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, UI_Combat.Instance.handFullSpawnCard.transform.position, Quaternion.identity);
        cardPrefab.transform.parent = UI_Combat.Instance.handFullSpawnCard.transform;
        cardPrefab.GetComponent<Canvas>().sortingOrder = 1000;
        ScriptableCard scriptableCard = cardScript.scriptableCard;

        //add the scriptable card object to the prefab class to reference
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        cardPrefab.GetComponent<CardScript>().cardID = cardScript.cardID;

        //make the local scale 1,1,1
        cardPrefab.transform.localScale = new Vector3(1, 1, 1);

        //update the information on the card prefab
        UpdateCardUI(cardPrefab);

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
        cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardUIObject;


    }


    public void DestroyPlayedCard(SystemManager.CardThrow cardThrow)
    {
        //instantiate the effect
        GameObject cardSoul;

        if (cardThrow == SystemManager.CardThrow.DISCARD)
        {
            //add it to the discard pile
            discardedPile.Add(savedPlayedCardScript);

            cardSoul = Instantiate(CombatCardHandler.Instance.discardEffect, savedPlayedCardScript.gameObject.transform.position, Quaternion.identity);
            //and assign the target
            cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardUIObject;
        }
        else if (cardThrow == SystemManager.CardThrow.BANISH)
        {
            //add it to the banished pile
            banishedPile.Add(savedPlayedCardScript);

            cardSoul = Instantiate(CombatCardHandler.Instance.banishEffect, savedPlayedCardScript.gameObject.transform.position, Quaternion.identity);
            //and assign the target
            cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.banishedUIObject;
        }

        //destroy it as we do not need it anymore
        Destroy(savedPlayedCardScript.gameObject);

        //rearrange hand
        HandManager.Instance.SetHandCards();
    }

    public void DestroyCardPrefab(CardScript cardScript, SystemManager.CardThrow cardThrow)
    {

        int index = -1;
        GameObject cardToDelete = null;

        //find the card object
        foreach (GameObject cardPrefab in HandManager.Instance.cardsInHandList)
        {
            //get the cardscript so we can compare the scriptable objects
            CardScript cardScriptFromGameObject = cardPrefab.GetComponent<CardScript>();

            //if they are equal then we destroy it
            if (cardScriptFromGameObject.cardID == cardScript.cardID)
            {
                //find the index of the card we are discarding
                index = HandManager.Instance.cardsInHandList.FindIndex(item => item.GetComponent<CardScript>().cardID == cardScript.cardID);

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
                cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.discardUIObject;
            }
            else if (cardThrow == SystemManager.CardThrow.BANISH)
            {
                cardSoul = Instantiate(CombatCardHandler.Instance.banishEffect, cardToDelete.transform.position, Quaternion.identity);
                //and assign the target
                cardSoul.GetComponent<EffectGoToTarget>().target = UI_Combat.Instance.banishedUIObject;
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

        CardScript tempCardScript = new CardScript();
        tempCardScript.scriptableCard = scriptableCard;

        AddCardToList(tempCardScript);

    }

    public void AddCardToList(CardScript cardScript)
    {

        if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.Hand)
        {

            if (handCards.Count >= HandManager.Instance.maxHandCardsLimit)
            {
                return;
            }

            //add to deck then draw it
            combatDeck.Add(cardScript);

            //get the index then draw it
            int combatDeckIndex = combatDeck.FindIndex(card => card.cardID == cardScript.cardID);

            DeckManager.Instance.GetCardFromCombatDeckToHand(combatDeckIndex);

        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.discardPile)
        {
            discardedPile.Add(cardScript);
        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.combatDeck)
        {
            combatDeck.Add(cardScript);
        }
        else if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.mainDeck)
        {
            StaticData.staticMainDeck.Add(cardScript);

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
