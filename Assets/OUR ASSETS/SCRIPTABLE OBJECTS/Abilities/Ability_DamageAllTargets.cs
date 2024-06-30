using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {

        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, null);

        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to all enemies";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, entity, null);
        GameObject[] targetsFound;
        //get all targets
        if (entity.tag == "Player")
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else 
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Player");
        }



        //then loop
        foreach (GameObject targetFound in targetsFound)
        {
            if (targetFound.GetComponent<EntityClass>().entityMode != SystemManager.EntityMode.DEAD) {
                int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, targetFound);
                CombatManager.Instance.AdjustTargetHealth(targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
            }
        }
      


    }


}
