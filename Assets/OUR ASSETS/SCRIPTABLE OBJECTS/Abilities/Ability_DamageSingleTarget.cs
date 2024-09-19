using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageSingleTarget", menuName = "CardAbility/Ability_DamageSingleTarget")]
public class Ability_DamageSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        try
        {
            int cardDmg = cardAbilityClass.abilityIntValue;
            int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, null);


            string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
            string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to an enemy";
            string final = keyword + " : " + description;

            return final;
        }
        catch (Exception ex)
        {
            Debug.LogError("Ability Description Error" + ex.Message);

            return "ERROR";
        }
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
            if (CombatCardHandler.Instance.targetClicked == null)
            {
                return;
            }
            else
            {
                realTarget = CombatCardHandler.Instance.targetClicked;
            }
        }

        base.OnPlayCard(cardScript, cardAbilityClass, entity, realTarget);


        ProceedToAbility(cardScript, cardAbilityClass, entity);



    }

    private void OnCompleteBase(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        // Proceed with animation and sound after the movement
        ProceedToAbility(cardScript, cardAbilityClass, entity);
    }

    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        //spawn prefab
        base.SpawnEffectPrefab(realTarget, cardAbilityClass);

        int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, realTarget);
        Combat.Instance.AdjustTargetHealth(realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
