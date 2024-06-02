using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_ShieldSingleTarget", menuName = "CardAbility/Ability_ShieldSingleTarget")]
public class Ability_ShieldSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Add " + GetAbilityVariable(cardScript) + " shield to character";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript)
    {
        base.OnPlayCard(cardScript);

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(cardScript), false, CombatManager.AdjustNumberMode.SHIELD);


    }


}
