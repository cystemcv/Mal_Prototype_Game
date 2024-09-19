using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_ShieldSingleTarget", menuName = "CardAbility/Ability_ShieldSingleTarget")]
public class Ability_ShieldSingleTarget : ScriptableCardAbility
{
    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;



    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Add " + cardAbilityClass.abilityIntValue + " shield to character";
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

        if (realTarget == null && entity != null)
        {
            realTarget = entity;
        }

        if (realTarget == null)
        {
            return;
        }

        base.OnPlayCard(cardScript, cardAbilityClass, entity, null);

        //spawn prefab
        base.SpawnEffectPrefab(realTarget, cardAbilityClass);



        Combat.Instance.AdjustTargetHealth(realTarget, cardAbilityClass.abilityIntValue, false, SystemManager.AdjustNumberModes.SHIELD);


    }


}
