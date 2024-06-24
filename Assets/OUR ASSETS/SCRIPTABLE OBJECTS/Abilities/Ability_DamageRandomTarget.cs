using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageRandomTarget", menuName = "CardAbility/Ability_DamageRandomTarget")]
public class Ability_DamageRandomTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    private GameObject targetFound;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, null);

        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to a random enemy";
        string final = keyword + " : " + description;

        return final;
    }




    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        //get all targets
        GameObject[] targetsFound;

        if (entity.tag == "Player")
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Enemy");
        }
        else
        {
            targetsFound = GameObject.FindGameObjectsWithTag("Player");
        }

        int randomNmbr = Random.Range(0, targetsFound.Length);

        targetFound = targetsFound[randomNmbr];

        base.OnPlayCard(cardScript, entity, targetFound);


        if (base.typeOfAttack == SystemManager.TypeOfAttack.MELLEE
           || base.typeOfAttack == SystemManager.TypeOfAttack.PROJECTILE)
        {
            InvokeHelper.Instance.Invoke(() => OnCompleteBase(cardScript, entity), base.timeToGetToTarget);

        }
        else
        {
            ProceedToAbility(cardScript, entity);
        }



    }

    private void OnCompleteBase(CardScript cardScript, GameObject entity)
    {
        // Proceed with animation and sound after the movement
        ProceedToAbility(cardScript, entity);
    }

    private void ProceedToAbility(CardScript cardScript, GameObject entity)
    {
        int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, targetFound);
        CombatManager.Instance.AdjustTargetHealth(targetFound, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
