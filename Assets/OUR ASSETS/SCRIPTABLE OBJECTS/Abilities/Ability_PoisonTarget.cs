using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_PoisonTarget", menuName = "CardAbility/Ability_PoisonTarget")]
public class Ability_PoisonTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to an enemy";
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

    private void ProceedToAbility(CardScript cardScript, GameObject character)
    {

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, 3);

    }

    //debuff

    public override bool OnEnemyTurnStart( GameObject target)
    {
        CombatManager.Instance.AdjustTargetHealth(target, 1, false, SystemManager.AdjustNumberModes.ATTACK);

        //activated
        return true;
    }


}
