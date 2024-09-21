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

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Heal " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " to character";
        string final = keyword + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, GameObject target)
    {
        //assign target 
        if (target != null)
        {
            realTarget = target;
        }
        else
        {
            realTarget = CombatCardHandler.Instance.targetClicked;
        }

        if (realTarget == null && entity != null)
        {
            realTarget = entity;
        }

        if (realTarget == null)
        {
            return;
        }

        base.OnPlayCard(cardScript, cardAbilityClass, entity, null);

        Combat.Instance.AdjustTargetHealth(realTarget, DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), false, SystemManager.AdjustNumberModes.HEAL);


    }


}
