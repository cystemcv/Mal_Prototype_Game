using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_HealSingleTarget", menuName = "CardAbility/Ability_HealSingleTarget")]
public class Ability_HealSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Heal " + GetAbilityVariable(cardScript) + " to character";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, entity, null);

        CombatManager.Instance.AdjustTargetHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.HEAL);


    }


}
