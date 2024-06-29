using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DiscardCardFromHand", menuName = "CardAbility/Ability_DiscardCardFromHand")]
public class Ability_DiscardCardFromHand : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Discard this card";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, entity, null);

       // Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = GetAbilityVariable(cardScript);

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
