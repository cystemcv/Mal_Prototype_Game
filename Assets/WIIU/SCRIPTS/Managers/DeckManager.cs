using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;

    public int turnHandCardsLimit = 5;
    public int maxHandCardsLimit = 10;

    //Deck
    public List<CardScript> deck;

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

    public float drawCardWaitTime = 0.6f;


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
    }

    public void BuildStartingDeck()
    {

        //loop throught each character on the character list and then add those cards to our deck
        foreach (ScriptablePlayer scriptablePlayer in CharacterManager.Instance.scriptablePlayer)
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
                    deck.Add(cardScript);
                }
            }

        }

    }

    public void DrawCardFromDeck()
    {

        //do not draw if both discard and deck pile is at 0

        if (deck.Count != 0 || discardedPile.Count != 0)
        {

            //check if there are cards on deck/ if not put the discard pile back to deck (loop)
            FillUpDeckFromDiscardPile();

            //put the top 1 card from deck on hand
            CardScript cardScript = deck[0];

            //if it passes the max hand limit the discard it, otherwise add it to the hand
            if (handCards.Count >= maxHandCardsLimit)
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
                InitializeCardPrefab(cardScript, UIManager.Instance.handObjectParent);

                //rearrange hand
                HandManager.Instance.SetHandCards();


            }

            //remove the top 1 card from deck
            deck.RemoveAt(0);


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

    public void DiscardCardFromHandRandom()
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = Random.Range(0, handCards.Count - 1);

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

        //activate all card abilities
        foreach (ScriptableCardAbility scriptableCardAbility in cardScript.scriptableCard.scriptableCardAbilities)
        {
            scriptableCardAbility.OnPlayCard(cardScript.scriptableCard);
        }

        ////the discard card
        DiscardCardFromHand(cardScript);

        //decrease available mana
        CombatManager.Instance.ManaAvailable -= cardScript.scriptableCard.primaryManaCost;
    }

    public string GenerateCardAbilityDescription(ScriptableCard scriptableCard)
    {
        string abilityDescription = "";

        //check if the card has any abilities to be played
        if (scriptableCard.scriptableCardAbilities.Count == 0)
        {
            return abilityDescription;
        }

        //activate all card abilities
        foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
        {
            abilityDescription += scriptableCardAbility.AbilityDescription(scriptableCard);
        }

        return abilityDescription;

    }

    public void PlayCardFromHandRandom()
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = Random.Range(0, handCards.Count - 1);

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

            DrawCardFromDeck();
            // Wait for 2 seconds
            yield return new WaitForSeconds(drawCardWaitTime);

         
        }

    }

    public void DiscardWholeHand()
    {

        ////add cards from hand to discard pile
        //foreach (ScriptableCard scriptableCard in handCards)
        //{
        //    discardedPile.Add(scriptableCard);
        //}

        ////destroy the gameobject prefab cards
        //foreach (Transform cardPrefab in UIManager.Instance.handObjectParent.transform)
        //{
        //    Destroy(cardPrefab.gameObject);
        //}

        ////clear hand list
        //handCards.Clear();

    }

    public void FillUpDeckFromDiscardPile()
    {
        if (deck.Count == 0)
        {

            //randomize list

            //add to deck and remove from discard pile
            foreach (CardScript cardScript in discardedPile)
            {
                Debug.Log("ID OF CARD DISCARDED : " + cardScript.cardID);
                deck.Add(cardScript);
            }

            //clear the discarded pile
            discardedPile.Clear();
        }
    }

    public bool CheckModeAvailabilityForCard(ScriptableCard scriptableCard)
    {
        bool accepted = false;

        //check if the card can be added based on the mode
        if (CharacterManager.Instance.scriptablePlayer.Count == 1 && scriptableCard.playerMode1 == true)
        {
            accepted = true;
        }
        else if (CharacterManager.Instance.scriptablePlayer.Count == 2 && scriptableCard.playerMode2 == true)
        {
            accepted = true;
        }
        else if (CharacterManager.Instance.scriptablePlayer.Count == 3 && scriptableCard.playerMode3 == true)
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
        deck.Add(null);
    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {

        // deck.Remove(card);

    }

    public void InitializeCardPrefab(CardScript cardScript, GameObject parent)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, UIManager.Instance.deckUIObject.transform.position, Quaternion.identity);
        //get the scriptable object
        ScriptableCard scriptableCard = cardScript.scriptableCard;

        //add the scriptable card object to the prefab class to reference
        cardPrefab.GetComponent<CardScript>().scriptableCard = scriptableCard;
        cardPrefab.GetComponent<CardScript>().cardID = cardScript.cardID;

        //set it as a child of the parent
        cardPrefab.transform.parent = parent.transform;

        //use the scriptable object to fill the art, text (title,desc,mana cost,etc)
        //for text USE TEXT MESH PRO

        UpdateCardUI(cardPrefab);

        //add it to the hand list
        HandManager.Instance.cardsInHandList.Add(cardPrefab);

        //check mana
        CombatManager.Instance.UpdateCardAfterManaChange(cardPrefab);

    }

    public void UpdateCardUI(GameObject cardPrefab)
    {
        ScriptableCard scriptableCard = cardPrefab.GetComponent<CardScript>().scriptableCard;
        Transform cardChild = cardPrefab.transform.GetChild(0);

        //for example
        cardChild.transform.Find("TitleBg").Find("TitleText").GetComponent<TMP_Text>().text = scriptableCard.cardName;
        cardChild.transform.Find("CardImage").GetComponent<Image>().sprite = scriptableCard.cardArt;
        cardChild.transform.Find("FlavorBg").Find("FlavorText").GetComponent<TMP_Text>().text = scriptableCard.cardFlavor;

        //mana cost
        cardChild.transform.Find("ManaBg").Find("ManaImage").Find("ManaText").GetComponent<TMP_Text>().text = scriptableCard.primaryManaCost.ToString();
        cardChild.transform.Find("ManaBg").Find("SecondaryManaImage").Find("SecondaryManaText").GetComponent<TMP_Text>().text = scriptableCard.primaryManaCost.ToString();

        //description is based on abilities
        cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text = "";
        foreach (ScriptableCardAbility scriptableCardAbility in scriptableCard.scriptableCardAbilities)
        {
            cardChild.transform.Find("DescriptionBg").Find("DescriptionText").GetComponent<TMP_Text>().text += scriptableCardAbility.AbilityDescription(scriptableCard) + "\n";
        }
        //activation should not be visible
        cardChild.transform.Find("Activation").GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

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

        //update the information on the card prefab
        UpdateCardUI(cardPrefab);

        //move it
        LeanTween.move(cardPrefab, UIManager.Instance.discardText.transform.position, 1.5f);
        LeanTween.scale(cardPrefab, new Vector3(0,0,0), 1.5f);
        Destroy(cardPrefab,1.5f);
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

            //destroy it as we do not need it anymore
            Destroy(cardToDelete);

            //rearrange hand
            HandManager.Instance.SetHandCards();
        }


    }


}
