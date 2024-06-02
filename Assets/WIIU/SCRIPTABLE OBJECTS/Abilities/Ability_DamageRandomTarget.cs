using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageRandomTarget", menuName = "CardAbility/Ability_DamageRandomTarget")]
public class Ability_DamageRandomTarget : ScriptableCardAbility
{

    // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to a random enemy";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript)
    {
        base.OnPlayCard(cardScript);

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int randomNmbr = Random.Range(0, enemies.Length);

        CombatManager.Instance.AdjustHealth(enemies[randomNmbr], GetAbilityVariable(cardScript), false, CombatManager.AdjustNumberMode.ATTACK);




    }


}
