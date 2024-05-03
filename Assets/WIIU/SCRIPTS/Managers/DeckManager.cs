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
    public List<ScriptableCard> deck;

    //Discarded Cards
    public List<ScriptableCard> discardedPile;

    //Banished Cards (Out of Play)
    public List<ScriptableCard> banishedPile;

    //Hand Cards 
    public List<ScriptableCard> handCards;



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
                    deck.Add(scriptableCard);
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
            ScriptableCard scriptableCard = deck[0];

            //if it passes the max hand limit the discard it, otherwise add it to the hand
            if (handCards.Count > maxHandCardsLimit) {

                //discard it
                discardedPile.Add(scriptableCard);
            }
            else
            {
                //add it to the hand
                handCards.Add(scriptableCard);
            }

            //remove the top 1 card from deck
            deck.RemoveAt(0);
        }

    }

    public void DiscardCardFromHand(ScriptableCard scriptableCard)
    {
        //add it to the discard pile
        discardedPile.Add(scriptableCard);

        //remove the card based on the index
        int index = handCards.IndexOf(scriptableCard);
        handCards.RemoveAt(index);

    }

    public void DiscardCardFromHandRandom()
    {
        if (handCards.Count == 0)
        {
            return;
        }

        //get the random card
        int randomIndex = Random.Range(0,handCards.Count-1);

        //call the discard function
        DiscardCardFromHand(handCards[randomIndex]);


    }

    public void StartTurnDrawCards()
    {
        //draw cards based on the limit (some relics might increase that limit)
        for (int i=0; i < turnHandCardsLimit; i++)
        {
            DrawCardFromDeck();
        }
    }

    public void DiscardWholeHand()
    {

        foreach (ScriptableCard scriptableCard in handCards)
        {
            discardedPile.Add(scriptableCard);
        }

        //clear hand list
        handCards.Clear();

    }

    public void FillUpDeckFromDiscardPile()
    {
        if (deck.Count == 0)
        {

            //randomize list

            //add to deck and remove from discard pile
            foreach (ScriptableCard scriptableCard in discardedPile)
            {
                deck.Add(scriptableCard);
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

    public void AddCardOnDeck(ScriptableCard card, int character)
    {
        deck.Add(card);
    }

    public void RemoveCardFromDeck(ScriptableCard card, int character)
    {

        deck.Remove(card);

    }

    public void InitializeCardOnPrefab(ScriptableCard card, GameObject parent)
    {

        //instantiate the prefab 
        // Instantiate at position (0, 0, 0) and zero rotation.
        GameObject cardPrefab = Instantiate(CardListManager.Instance.cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        //set it as a child of the parent
        cardPrefab.transform.parent = parent.transform;

        //use the scriptable object to fill the art, text (title,desc,mana cost,etc)
        //for text USE TEXT MESH PRO
        //for example
        cardPrefab.transform.Find("ImageChild").GetComponent<Image>().sprite = card.cardArt;

    }

}
