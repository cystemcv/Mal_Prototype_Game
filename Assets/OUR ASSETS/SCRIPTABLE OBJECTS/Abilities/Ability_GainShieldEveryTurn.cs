using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_GainShieldEveryTurn", menuName = "CardAbility/Ability_GainShieldEveryTurn")]
public class Ability_GainShieldEveryTurn : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int buffLifeTime = 0;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Gain " + GetAbilityVariable(cardScript) + " shield " + buffLifeTime + " turns";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {


        base.OnPlayCard(cardScript, entity, CombatManager.Instance.targetClicked);


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

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, CombatManager.Instance.targetClicked, buffLifeTime);

        //get the buff or debuff to do things
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(CombatManager.Instance.targetClicked, this.scriptableBuffDebuff.nameID);
        buffDebuffClass.tempVariable = GetAbilityVariable(cardScript);


        ////increase character attack
        //if (CombatManager.Instance.targetClicked.tag == "Player")
        //{

        //}
        //else if(CombatManager.Instance.targetClicked.tag == "Enemy")
        //{

        //}




    }

    //debuff

    public override bool OnCharacterTurnEnd(GameObject target)
    {
        //return base.OnCharacterTurnEnd(target);

        //give shield to target

            //get buff temp variable
            //get the buff or debuff to do things
            BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.scriptableBuffDebuff.nameID);

            CombatManager.Instance.AdjustTargetHealth(target, buffDebuffClass.tempVariable, false, SystemManager.AdjustNumberModes.SHIELD);


        return true;
    }

    //public override bool OnCharacterTurnStart( GameObject target)
    //{

    //    //activated
    //    //return true;
    //}

    //public override void OnExpireBuffDebuff(GameObject target)
    //{
    //    //get the buff/debuff
    //    BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(target, this.scriptableBuffDebuff.nameID);

    //    if (target.tag == "Player")
    //    {
    //        target.GetComponent<CharacterClass>().attack -= buffDebuffClass.tempVariable;
    //    }
    //    else if (target.tag == "Enemy")
    //    {

    //    }

    //}




}
