using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamagePierceSingleTarget", menuName = "CardAbility/Ability_DamagePierceSingleTarget")]
public class Ability_DamagePierceSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        int cardDmg = cardAbilityClass.abilityIntValue;
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, null);

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to an enemy (Ignore Block)";
        string final = keyword + " : " + description;

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

        base.OnPlayCard(cardScript, cardAbilityClass, entity, realTarget);



            ProceedToAbility(cardScript, cardAbilityClass, entity);


    }


    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, realTarget);
        Combat.Instance.AdjustTargetHealth(realTarget, calculatedDmg, true, SystemManager.AdjustNumberModes.ATTACK);

    }


}
