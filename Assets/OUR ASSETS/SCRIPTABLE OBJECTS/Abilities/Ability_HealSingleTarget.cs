using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_HealSingleTarget", menuName = "CardAbility/Ability_HealSingleTarget")]
public class Ability_HealSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Heal " + GetAbilityVariable(cardScript) + " to character";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript)
    {
        base.OnPlayCard(cardScript);

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(cardScript), false, CombatManager.AdjustNumberMode.HEAL);


    }


}