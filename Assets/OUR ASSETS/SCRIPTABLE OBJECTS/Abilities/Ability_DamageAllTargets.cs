using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_DamageAllTargets", menuName = "CardAbility/Ability_DamageAllTargets")]
public class Ability_DamageAllTargets : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass,GameObject entity)
    {

        int cardDmg = cardAbilityClass.abilityIntValue;
        int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, null);

        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to all enemies";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, cardAbilityClass, entity, null);
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

                //spawn prefab
                base.SpawnEffectPrefab(targetFound, cardAbilityClass);

                int calculatedDmg = Combat.Instance.CalculateEntityDmg(cardAbilityClass.abilityIntValue, entity, targetFound);
                Combat.Instance.AdjustTargetHealth(targetFound, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);
            }
        }
      


    }


}
