using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageSingleTarget", menuName = "CardAbility/Ability_DamageSingleTarget")]
public class Ability_DamageSingleTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;

    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, null);
    

        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to an enemy";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        //assign target 
        if (target != null)
        {
            realTarget = target;
        }
        else
        {
            realTarget = CombatManager.Instance.targetClicked;
        }

        base.OnPlayCard(cardScript, entity, realTarget);



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
        int calculatedDmg = CombatManager.Instance.CalculateEntityDmg(GetAbilityVariable(cardScript), entity, realTarget);
        CombatManager.Instance.AdjustTargetHealth(realTarget, calculatedDmg, false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
