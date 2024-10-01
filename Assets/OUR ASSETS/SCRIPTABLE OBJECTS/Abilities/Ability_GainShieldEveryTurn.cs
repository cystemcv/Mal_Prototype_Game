using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_GainShieldEveryTurn", menuName = "CardAbility/Ability_GainShieldEveryTurn")]
public class Ability_GainShieldEveryTurn : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int buffLifeTime = 0;
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Gain " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " shield " + buffLifeTime + " turns";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
  
            realTarget = CombatCardHandler.Instance.targetClicked;
     

        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);


  
            ProceedToAbility(cardScript, cardAbilityClass, entity);
 


    }



    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, buffLifeTime);

        //get the buff or debuff to do things
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);
        buffDebuffClass.tempVariable = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);


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

        Combat.Instance.AdjustTargetHealth(target, buffDebuffClass.tempVariable, false, SystemManager.AdjustNumberModes.SHIELD);


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
