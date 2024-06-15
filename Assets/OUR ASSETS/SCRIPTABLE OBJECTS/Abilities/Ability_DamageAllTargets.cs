using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to all enemies";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript, GameObject character)
    {
        base.OnPlayCard(cardScript,character);

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //then loop
        foreach (GameObject enemy in enemies)
        {
            CombatManager.Instance.AdjustHealth(enemy, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.ATTACK);
        }
      


    }


}
