using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DiscardCardFromHand", menuName = "CardAbility/Ability_DiscardCardFromHand")]
public class Ability_DiscardCardFromHand : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Discard this card";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);

       // Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
