using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_PoisonTarget", menuName = "CardAbility/Ability_PoisonTarget")]
public class Ability_PoisonTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Deal " + GetAbilityVariable(scriptableCard) + " to an enemy";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();
        BuffSystemManager.Instance.AddDebuffToEnemyTarget(this, CombatManager.Instance.targetClicked, 3);

    }

    //debuff

    public override bool OnEnemyTurnStart( GameObject target)
    {
        CombatManager.Instance.AdjustHealth(target, 1, false, CombatManager.AdjustNumberMode.ATTACK);

        //activated
        return true;
    }


}
