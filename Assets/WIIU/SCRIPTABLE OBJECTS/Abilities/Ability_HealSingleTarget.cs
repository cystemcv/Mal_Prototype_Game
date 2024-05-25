using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_HealSingleTarget", menuName = "CardAbility/Ability_HealSingleTarget")]
public class Ability_HealSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Heal " + GetAbilityVariable(scriptableCard) + " to character";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(scriptableCard), false, CombatManager.AdjustNumberMode.HEAL);


    }


}
