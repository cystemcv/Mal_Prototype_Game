using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ChooseACardFromList", menuName = "CardAbility/Ability_ChooseACardFromList")]
public class Ability_ChooseACardFromList : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public SystemManager.MainClass mainClass;
    public SystemManager.CardType cardType;
    public SystemManager.AddCardTo addCardTo;
    public bool setManaCost = false;
    public bool modifyManaCost = false;
    public int cardsToChoose = 3;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
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



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        try { 
        base.OnPlayCard(cardScript, entity, null);

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
        UI_Combat.Instance.chooseACardScreen.SetActive(true);

        //change the mode
        SystemManager.Instance.abilityMode = SystemManager.AbilityModes.CHOICE;

        //where to add card
        SystemManager.Instance.addCardTo = SystemManager.AddCardTo.Hand;

            //get the cards to display
            for (int i = 0; i < cardsToChoose; i++)
            {
                //get the random card from the filtered list
                int randomIndex = UnityEngine.Random.Range(0, filteredCardList.Count);

                //go for next item
                if (randomIndex == -1)
                {
                    continue;
                }

                //generate cardScript
                CardScript cardScriptTemp = new CardScript();
                cardScriptTemp.scriptableCard = filteredCardList[randomIndex];



                if (setManaCost)
                {
                    cardScriptTemp.primaryManaCost = GetAbilityVariable(cardScript);

                    cardScriptTemp.resetManaCost = true;
                    cardScriptTemp.changedMana = true;
                }
                else if (modifyManaCost)
                {
                    cardScriptTemp.primaryManaCost += GetAbilityVariable(cardScript);
                    //reset to 0 if its below
                    if (cardScriptTemp.primaryManaCost <= 0)
                    {
                        cardScriptTemp.primaryManaCost = 0;
                    }

                    cardScriptTemp.resetManaCost = true;
                    cardScriptTemp.changedMana = true;
                }


                //generate the card and parent it
                DeckManager.Instance.InitializeCardPrefab(cardScriptTemp, UI_Combat.Instance.chooseACardScreen.transform.Find("CardContainer").gameObject, false);
            }
         


        }
        catch (Exception ex)
        {
            Debug.LogError("Ability Error : " + ex.Message);
        }

    }




}
