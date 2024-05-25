using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ShieldSingleTarget", menuName = "CardAbility/Ability_ShieldSingleTarget")]
public class Ability_ShieldSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Add " + GetAbilityVariable(scriptableCard) + " shield to character";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(scriptableCard), false, CombatManager.AdjustNumberMode.SHIELD);


    }


}
