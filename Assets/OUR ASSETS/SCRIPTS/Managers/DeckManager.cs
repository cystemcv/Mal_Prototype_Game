using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;



    //Deck
    public List<CardScript> mainDeck;
    public List<CardScript> combatDeck;

    //Discarded Cards
    public List<CardScript> discardedPile;

    //Banished Cards (Out of Play)
    public List<CardScript> banishedPile;

    //Hand Cards 
    public List<CardScript> handCards;

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

        DeckManager.Instance.BuildStartingDeck();
    }

    public void BuildStartingDeck()
    {

        // Clear the deck before building a new one
        mainDeck.Clear();

        //loop throught each character on the character list and then add those cards to our deck
        foreach (ScriptablePlayer scriptablePlayer in CharacterManager.Instance.scriptablePlayerList)
        {

            //loop for each starting card list and add it to our deck
            foreach (ScriptableCard scriptableCard in scriptablePlayer.startingCards)
            {

                //add the card on the deck depending on the mode
                if (CheckModeAvailabilityForCard(scriptableCard))
                {


                    //add cards to the deck
                    CardScript cardScript = new CardScript();
                    cardScript.scriptableCard = scriptableCard;
                    mainDeck.Add(cardScript);
                }
            }

        }

        // Shuffle the deck
        ShuffleDeck(mainDeck);

    }

    public void ShuffleDeck(List<CardScript> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardScript temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void DrawCardFromDeck()
    {

        //do not draw if both discard and deck pile is at 0

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

                InitializeCardPrefabDiscard(cardScript);
            }
            else
            {

                //add it to the hand
                handCards.Add(cardScript);

                //instantiate the card
                InitializeCardPrefab(cardScript, UIManager.Instance.handObjectParent, true);

                //rearrange hand
                HandManager.Instance.SetHandCards();


            }

            //remove the top 1 card from deck
            combatDeck.RemoveAt(0);


        }

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
            InitializeCardPrefab(cardScript, UIManager.Instance.handObjectParent, true);

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
        DestroyCardPrefab(cardScript);



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
        int randomIndex = Random.Range(0, handCards.Count);

        //call the discard function
        DiscardCardFromHand(handCards[randomIndex]);




    }

    public void PlayCard(CardScript cardScript)
    {
        //check if the card has any abilities to be played
        if (cardScript.scriptableCard.scriptableCardAbilities.Count == 0)
        {
            return;
        }

        //save the cardScript temp
        CardScript tempCardScript = new CardScript();
        tempCardScript = cardScript;

        ////the discard card
        DiscardCardFromHand(cardScript);

        //decrease available mana
        CombatManager.Instance.ManaAvailable -= cardScript.primaryManaCost;

        //activate all card abilities

        StartCoroutine(PlayCardCoroutine(tempCardScript));


    }

    IEnumerator PlayCardCoroutine(CardScript cardScript)
    {

        SystemManager.Instance.thereIsActivatedCard = true;

        //get the character to be used
        GameObject character = CombatManager.Instance.GetTheCharacterThatUsesTheCard(cardScript);

        foreach (ScriptableCardAbility scriptableCardAbility in cardScript.scriptableCard.scriptableCardAbilities)
        {



            // Wait for 2 seconds
            scriptableCardAbility.OnPlayCard(cardScript, character, null);

            //check to reset mana to the original cost if neeeded
            if (cardScript.resetManaCost)
            {
                cardScript.primaryManaCost = cardScript.scriptableCard.primaryManaCost;
                cardScript.resetManaCost = false;
                cardScript.changedMana = false;
            }

            float waitAmount = scriptableCardAbility.waitForAbility;
            if (scriptableCardAbility.runToTarget)
            {
                waitAmount += scriptableCardAbility.timeToGetToTarget;
            }

            //add also a small delay
            waitAmount += 0.2f;

             yield return new WaitForSeconds(waitAmount);
            //yield return new WaitForSeconds(10f);
        }

        //go back to idle animation
        Animator animator = character.transform.Find("model").GetComponent<Animator>();

        if (animator != null)
        {
            Debug.Log("idle?");
            animator.SetTrigger("Idle");
        }

        //card ended
        SystemManager.Instance.thereIsActivatedCard = false;

    }

    //public string GenerateCardAbilityDescription(CardScript cardScript)
    //{
    //    string abilityDescription = "";

    //    //check if the card has any abilities to be played
    //    if (cardScript.scriptableCard.scriptableCardAbilities.Count == 0)
    //    {
    //        return abilityDescription;
    //    }

    //    //activate all card abilities
    //    foreach (ScriptableCardAbility scriptableCardAbility in cardScript.scriptableCard.scriptableCardAbilities)
    //    {
    //        abilityDescription += scriptableCardAbility.AbilityDescription(cardScript);
    //    }

    //    return abilityDescription;

    //}

    public void PlayCardFromHandRandom()
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = Random.Range(0, handCards.Count);

        ////test the desc
        //Debug.Log(GenerateCardAbilityDescription(handCards[randomIndex]));

        ////play the card
        //PlayCard(handCards[randomIndex]);

        //call the discard function
        DiscardCardFromHand(handCards[randomIndex]);


    }

    public void DrawMultipleCards(int numberOfCards)
    {
        //draw cards
        StartCoroutine(DrawMultipleCardsCoroutine(numberOfCards));
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
                Debug.Log("ID OF CARD DISCARDED : " + cardScript.cardID);
                combatDeck.Add(cardScript);
            }

            //clear the discarded pile
            discardedPile.Clear();
        }

        ShuffleDeck(combatDeck);
    }

    public bool CheckModeAvailabilityForCard(ScriptableCard scriptableCard)
    {
        bool accepted = false;

        //check if the card can be added based on the mode
        if (CharacterManager.Instance.scriptablePlayerList.Count == 1 && scriptableCard.playerMode1 == true)
        {
            accepted = true;
        }
        else if (CharacterManager.Instance.scriptablePlayerList.Count == 2 && scriptableCard.playerMode2 == true)
        {
            accepted = true;
        }
        else if (CharacterManager.Instance.scriptablePlayerList.Count == 3 && scriptableCard.playerMode3 == true)
        {
            accepted = true;
        }


        return accepted;
    }

    public void AddCardOnDeck(ScriptableCard scriptableCard, int character)
    {

        //add a card ti deck
        GameObject cardPrefab = CardListManager.Instance.cardPrefab;
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        mainDeck.Add(null);
    }

    public void AddCardOnCombatDeck(CardScript cardScript)
    {

    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {

        // deck.Remove(card);

    }

    public void InitializeCardPrefab(CardScript cardScript, GameObject parent, bool addToHand)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, UIManager.Instance.deckUIObject.transform.position, Quaternion.identity);
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

        //make the local scale 1,1,1
        cardPrefab.transform.localScale = new Vector3(1, 1, 1);

        //use the scriptable object to fill the art, text (title,desc,mana cost,etc)
        //for text USE TEXT MESH PRO

        UpdateCardUI(cardPrefab);

        //add sorting 
        cardPrefab.GetComponent<CardEvents>().sortOrder = 1200;
        cardPrefab.GetComponent<Canvas>().sortingOrder = 1200;

        //add it to the hand list
        if (addToHand)
        {
            HandManager.Instance.cardsInHandList.Add(cardPrefab);
        }

        //check mana
        CombatManager.Instance.UpdateCardAfterManaChange(cardPrefab);

    }

    public void UpdateCardUI(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        CardScript cardScript = cardPrefab.GetComponent<CardScript>();
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //get the character to be used
        GameObject character = CombatManager.Instance.GetTheCharacterThatUsesTheCard(cardScript);

        //for example
        cardChild.transform.Find("TitleBg").Find("TitleText").GetComponent<TMP_Text>().text = scriptableCard.cardName;
        cardChild.transform.Find("CardImage").GetComponent<Image>().sprite = scriptableCard.cardArt;
        cardChild.transform.Find("TypeBg").Find("TypeText").GetComponent<TMP_Text>().text = scriptableCard.cardType.ToString();

        cardChild.transform.Find("MainBg").GetComponent<Image>().color = CardListManager.Instance.GetClassColor(scriptableCard.mainClass);

        //mana cost
        cardChild.transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>().text = cardScript.primaryManaCost.ToString();
        //cardChild.transform.Find("ManaBg").Find("SecondaryManaImage").Find("SecondaryManaText").GetComponent<TMP_Text>().text = scriptableCard.primaryManaCost.ToString();

        //description is based on abilities
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "";
        foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += scriptableCardAbility.AbilityDescription(cardScript, character) + "\n";
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
        GameObject character = CombatManager.Instance.GetTheCharacterThatUsesTheCard(cardScript);
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "";
        foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += scriptableCardAbility.AbilityDescription(cardScript, character) + "\n";
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

    public void InitializeCardPrefabDiscard(CardScript cardScript)
    {
        //create gameobject on scene and spawn it on the discard spawner
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, UIManager.Instance.discardUISpawn.transform.position, Quaternion.identity);
        cardPrefab.transform.parent = UIManager.Instance.discardUISpawn.transform;
        cardPrefab.GetComponent<Canvas>().sortingOrder = 1000;
        ScriptableCard scriptableCard = cardScript.scriptableCard;

        //add the scriptable card object to the prefab class to reference
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        cardPrefab.GetComponent<CardScript>().cardID = cardScript.cardID;

        //make the local scale 1,1,1
        cardPrefab.transform.localScale = new Vector3(1, 1, 1);

        //update the information on the card prefab
        UpdateCardUI(cardPrefab);

        //move it
        LeanTween.move(cardPrefab, UIManager.Instance.discardText.transform.position, 1.5f);
        //LeanTween.scale(cardPrefab, new Vector3(0,0,0), 1.5f);
        Destroy(cardPrefab, 1.5f);
    }

    public void DestroyCardPrefab(CardScript cardScript)
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
            GameObject cardSoul = Instantiate(CombatManager.Instance.particleCardSoul, cardToDelete.transform.position, Quaternion.identity);

            //and assign the target
            cardSoul.GetComponent<EffectGoToTarget>().target = UIManager.Instance.discardText;

            //destroy it as we do not need it anymore
            Destroy(cardToDelete);

            //rearrange hand
            HandManager.Instance.SetHandCards();
        }


    }

    public void AddCardToList(CardScript cardScript)
    {

        if (SystemManager.Instance.addCardTo == SystemManager.AddCardTo.Hand)
        {
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
            mainDeck.Add(cardScript);
        }

        //close the thing 
        UIManager.Instance.chooseACardScreen.SetActive(false);

        //resume
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.NONE;

        SystemManager.Instance.DestroyAllChildren(UIManager.Instance.chooseACardScreen.transform.Find("CardContainer").gameObject);

    }




}
