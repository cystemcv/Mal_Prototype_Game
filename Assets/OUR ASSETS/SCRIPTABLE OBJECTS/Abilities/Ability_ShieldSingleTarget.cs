using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ShieldSingleTarget", menuName = "CardAbility/Ability_ShieldSingleTarget")]
public class Ability_ShieldSingleTarget : ScriptableCardAbility
{
    [Header("UNIQUE")]
     public int empty;

    private GameObject realTarget;



    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Add " + GetAbilityVariable(cardScript) + " shield to character";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
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

        base.OnPlayCard(cardScript, entity, null);



        Combat.Instance.AdjustTargetHealth(realTarget, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.SHIELD);


    }


}
