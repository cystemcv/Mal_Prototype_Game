using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_HealSingleTarget", menuName = "CardAbility/Ability_HealSingleTarget")]
public class Ability_HealSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;
    private GameObject realTarget;
    private GameObject entityUsedCard;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Heal " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " to character";
        string final = keyword + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target 

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCard = entity;


        if (realTarget == null && entity != null)
        {
            realTarget = entity;
        }

        if (realTarget == null)
        {
            return;
        }

        base.OnPlayCard(cardScript, cardAbilityClass, entityUsedCard, controlBy);

        Combat.Instance.AdjustTargetHealth(entityUsedCard, realTarget, DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), false, SystemManager.AdjustNumberModes.HEAL);


    }


}
