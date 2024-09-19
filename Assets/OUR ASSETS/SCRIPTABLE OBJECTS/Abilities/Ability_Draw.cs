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
        string description = "Draw " + cardAbilityClass.abilityIntValue + " cards";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, null);

       // Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = cardAbilityClass.abilityIntValue;

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
