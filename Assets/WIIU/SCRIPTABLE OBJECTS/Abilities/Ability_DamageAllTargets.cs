using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Deal " + GetAbilityVariable(scriptableCard) + " to all enemies";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //then loop
        foreach (GameObject enemy in enemies)
        {
            CombatManager.Instance.AdjustHealth(enemy, GetAbilityVariable(scriptableCard), false);
        }
      


    }


}
