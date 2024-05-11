using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_Damage", menuName = "CardAbility/Ability_Damage")]
public class Ability_Damage : ScriptableCardAbility
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

        Debug.Log("Deal " + GetAbilityVariable(scriptableCard) +  " damage to target " + CombatManager.Instance.targetClicked.name);

    }


}
