using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_GetCardFromDeck", menuName = "CardAbility/Ability_GetCardFromDeck")]
public class Ability_GetCardFromDeck : ScriptableCardAbility
{

    public SystemManager.CardType cardType;
    public bool setManaCost = false;
    public bool modifyManaCost = false;


    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Find a random " + cardType + " card and add it to your hand";
        if (setManaCost)
        {
            description += "(Mana Cost is set to " + GetAbilityVariable(cardScript) + " )";
        }
        else if (modifyManaCost)
        {
            if (GetAbilityVariable(cardScript) > 0)
            {
                description += "(Increase Mana Cost by " + GetAbilityVariable(cardScript) + " )";
            }
            else if(GetAbilityVariable(cardScript) < 0)
            {
                description += "(Decrease Mana Cost by " + GetAbilityVariable(cardScript) + " )";
            }
        }

        
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {
        base.OnPlayCard(cardScript,character, null);
        //based on the type
        // Filter the combatDeck to get only the cards of the specified type
        List<CardScript> filteredDeck = DeckManager.Instance.combatDeck.Where(card => card.scriptableCard.cardType == cardType).ToList();

        // Check if there are any cards of the specified type
        if (filteredDeck.Count == 0)
        {
            Debug.LogWarning("No cards of the specified type found in the deck.");
            return;
        }


        // Select a random card from the filtered list
        int randomIndex = Random.Range(0, filteredDeck.Count);

        CardScript foundFilteredCard = filteredDeck[randomIndex];

        //find the index of the card we are discarding
        int combatDeckIndex = DeckManager.Instance.combatDeck.FindIndex(card => card.cardID == foundFilteredCard.cardID);

        //get the actual card from combat deck
        CardScript foundCardScript = DeckManager.Instance.combatDeck[combatDeckIndex];

        if (DeckManager.Instance.handCards.Count >= HandManager.Instance.maxHandCardsLimit)
        {

            //discard it
            DeckManager.Instance.discardedPile.Add(cardScript);

            DeckManager.Instance.InitializeCardPrefabDiscard(cardScript);

            return;
        }

        if (setManaCost)
        {
            foundCardScript.primaryManaCost = GetAbilityVariable(cardScript);

            foundCardScript.resetManaCost = true;
            foundCardScript.changedMana = true;
        }
        else if (modifyManaCost)
        {
            foundCardScript.primaryManaCost += GetAbilityVariable(cardScript);
            //reset to 0 if its below
            if (foundCardScript.primaryManaCost <= 0)
            {
                foundCardScript.primaryManaCost = 0;
            }

            foundCardScript.resetManaCost = true;
            foundCardScript.changedMana = true;
        }


        Debug.Log("card 21s " + foundCardScript.changedMana);
 
    
        DeckManager.Instance.GetCardFromCombatDeckToHand(combatDeckIndex);


    }




}
