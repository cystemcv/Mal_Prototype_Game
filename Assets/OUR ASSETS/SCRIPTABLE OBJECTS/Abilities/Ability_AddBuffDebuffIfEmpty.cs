using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_AddBuffDebuffIfEmpty", menuName = "CardAbility/Ability_AddBuffDebuffIfEmpty")]
public class Ability_AddBuffDebuffIfEmpty : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;
    private GameObject realTarget;
    private GameObject entityUsedCard;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string description = "";

        description = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " <color=yellow>" + this.scriptableBuffDebuff.nameID + "</color> ";
        description += (infiniteDuration) ? "" : " for " + DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList) + " turns";
        description += " if there is no " + this.scriptableBuffDebuff.nameID + " on target!";

        string final = description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target \

    

        realTarget = CombatCardHandler.Instance.targetClicked;
        entityUsedCard = entity;

        BuffDebuffClass buffDebuff = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);

        if (buffDebuff == null)
        {
            base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);

            //BuffSystemManager.Instance.AddBuffDebuff(realTarget, this, DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList), DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList));

        }


    }



    //debuff

    //public override bool OnEnemyTurnStart(GameObject target)
    //{
    //    Combat.Instance.AdjustTargetHealth(entityUsedCard, target, 1, false, SystemManager.AdjustNumberModes.ATTACK);

    //    //activated
    //    return true;
    //}


}
