using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_BanishCardFromHand", menuName = "CardAbility/Ability_BanishCardFromHand")]
public class Ability_BanishCardFromHand : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Banish this card";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);

        DeckManager.Instance.DestroyPlayedCard(SystemManager.CardThrow.BANISH);

        //DeckManager.Instance.BanishCardFromHand(cardScript);

    }




}
