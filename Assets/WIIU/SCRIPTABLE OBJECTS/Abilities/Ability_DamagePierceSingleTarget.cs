using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamagePierceSingleTarget", menuName = "CardAbility/Ability_DamagePierceSingleTarget")]
public class Ability_DamagePierceSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Deal " + GetAbilityVariable(scriptableCard) + " to an enemy (Ignore Block)";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(scriptableCard), true);


    }


}
