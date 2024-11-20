using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_IncreaseDefenceTimer", menuName = "CardAbility/Ability_IncreaseDefenceTimer")]
public class Ability_IncreaseDefenceTimer : ScriptableCardAbility
{

    [Header("UNIQUE")]
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        string description = "";

        description = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " <color=yellow>Defence</color> ";
        description += (infiniteDuration) ? "" : " for " + DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList) + " turns";

        string final =  description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target 

        realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);


        ProceedToAbility(cardScript, cardAbilityClass, entity);



    }

    private void ProceedToAbility(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList));

        //get the buff or debuff to do things
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);

        //increase the variable that will store the temp attack
        buffDebuffClass.tempVariable += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);


        //increase character attack
        EntityClass entityClass = realTarget.GetComponent<EntityClass>();
        entityClass.defence += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);





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

        target.GetComponent<EntityClass>().defence -= buffDebuffClass.tempVariable;


    }




}
