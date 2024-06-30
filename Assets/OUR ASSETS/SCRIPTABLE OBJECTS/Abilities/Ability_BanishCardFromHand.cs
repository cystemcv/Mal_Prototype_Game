using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_BanishCardFromHand", menuName = "CardAbility/Ability_BanishCardFromHand")]
public class Ability_BanishCardFromHand : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Banish this card";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, entity, null);

        DeckManager.Instance.DestroyPlayedCard(SystemManager.CardThrow.BANISH);

        //DeckManager.Instance.BanishCardFromHand(cardScript);

    }




}
