using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_PoisonTarget", menuName = "CardAbility/Ability_PoisonTarget")]
public class Ability_PoisonTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to an enemy";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject character)
    {
        base.OnPlayCard(cardScript, character);
        BuffSystemManager.Instance.AddDebuffToEnemyTarget(this, CombatManager.Instance.targetClicked, 3);

    }

    //debuff

    public override bool OnEnemyTurnStart( GameObject target)
    {
        CombatManager.Instance.AdjustHealth(target, 1, false, SystemManager.AdjustNumberModes.ATTACK);

        //activated
        return true;
    }


}
