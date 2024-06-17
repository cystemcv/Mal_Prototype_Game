using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_HealSingleTarget", menuName = "CardAbility/Ability_HealSingleTarget")]
public class Ability_HealSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {
        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Heal " + GetAbilityVariable(cardScript) + " to character";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {
        base.OnPlayCard(cardScript,character, null);

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.HEAL);


    }


}
