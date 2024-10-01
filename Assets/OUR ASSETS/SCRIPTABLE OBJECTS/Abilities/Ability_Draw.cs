using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_Draw", menuName = "CardAbility/Ability_Draw")]
public class Ability_Draw : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass,GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Draw " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " cards";
        string final = keyword + description;

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
