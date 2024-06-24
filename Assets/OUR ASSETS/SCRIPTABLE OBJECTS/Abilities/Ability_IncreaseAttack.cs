using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_IncreaseAttack", menuName = "CardAbility/Ability_IncreaseAttack")]
public class Ability_IncreaseAttack : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int buffLifeTime = 0;
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "";
        if (GetAbilityVariable(cardScript) < 0)
        {
            description = "Decrease Attack by " + Mathf.Abs(GetAbilityVariable(cardScript)) + " for " + buffLifeTime + " turns";
        }
        else
        {
            description = "Increase Attack by " + GetAbilityVariable(cardScript) + " for " + buffLifeTime + " turns";
        }


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

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, 3);

        //get the buff or debuff to do things
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);

        //increase the variable that will store the temp attack
        buffDebuffClass.tempVariable += GetAbilityVariable(cardScript);

        //increase character attack
        EntityClass entityClass = realTarget.GetComponent<EntityClass>();
        entityClass.attack += GetAbilityVariable(cardScript);





    }

    //debuff

    public override bool OnCharacterTurnStart(GameObject target)
    {

        //activated
        return true;
    }

    public override void OnExpireBuffDebuff(GameObject target)
    {
        //get the buff/debuff
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.scriptableBuffDebuff.nameID);

        target.GetComponent<EntityClass>().attack -= buffDebuffClass.tempVariable;


    }




}
