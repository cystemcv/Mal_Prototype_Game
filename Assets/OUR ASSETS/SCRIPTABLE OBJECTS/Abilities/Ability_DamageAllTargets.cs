using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {

        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, null);

        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to all enemies";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {
        base.OnPlayCard(cardScript,character, null);

        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");


        //then loop
        foreach (GameObject enemy in enemies)
        {
            int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, enemy);
            CombatManager.Instance.AdjustHealth(enemy, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
        }
      


    }


}
