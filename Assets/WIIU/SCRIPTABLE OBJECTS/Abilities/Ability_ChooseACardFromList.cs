using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ChooseACardFromList", menuName = "CardAbility/Ability_ChooseACardFromList")]
public class Ability_ChooseACardFromList : ScriptableCardAbility
{
    public CharacterManager.MainClass mainClass;
    public CardListManager.CardType cardType;
    public bool setManaCost = false;
    public bool modifyManaCost = false;
    public int cardsToChoose = 3;

    public bool addToHand = false;
    public bool addToDeck = false;
    public bool addToDiscardPile = false;

    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Choose a " + mainClass + " " + cardType + " card and add it to your hand";
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
            else if (GetAbilityVariable(cardScript) < 0)
            {
                description += "(Decrease Mana Cost by " + GetAbilityVariable(cardScript) + " )";
            }
        }


        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript)
    {
        base.OnPlayCard(cardScript);

        List<ScriptableCard> listToChoose;

        //go throught the list and filter it based on our criteria
        List<ScriptableCard> filteredCardList = CardListManager.Instance.cardPool.Where(card =>
        card.cardType == cardType
        && card.mainClass == mainClass
        ).ToList();

        if (filteredCardList.Count == 0)
        {
            return;
        }

        //display screen
        UIManager.Instance.chooseACardScreen.SetActive(true);

        //change the mode
        CombatManager.Instance.abilityMode = CombatManager.AbilityModes.CHOICE;

        //get the cards to display
        for (int i = 0; i < cardsToChoose; i++)
        {
            //get the random card from the filtered list
            int randomIndex = Random.Range(0, filteredCardList.Count);

            //go for next item
            if (randomIndex == -1)
            {
                continue;
            }

            //generate cardScript
            CardScript cardScriptTemp = new CardScript();
            cardScriptTemp.scriptableCard = filteredCardList[randomIndex];

    

            //generate the card and parent it
            DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UIManager.Instance.chooseACardScreen.transform.Find("CardContainer").gameObject, false, false);



        }

    }




}
