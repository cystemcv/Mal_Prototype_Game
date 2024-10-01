using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_IncreaseAttack", menuName = "CardAbility/Ability_IncreaseAttack")]
public class Ability_IncreaseAttack : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int buffLifeTime = 0;
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "";
        if (DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) < 0)
        {
            description = "Decrease Attack by " + Mathf.Abs(DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList)) + " for " + buffLifeTime + " turns";
        }
        else
        {
            description = "Increase Attack by " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " for " + buffLifeTime + " turns";
        }


        string final = keyword + " : " + description;

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

        BuffSystemManager.Instance.AddBuffDebuffToTarget(this, realTarget, 3);

        //get the buff or debuff to do things
        BuffDebuffClass buffDebuffClass = BuffSystemManager.Instance.GetBuffDebuffClassFromTarget(realTarget, this.scriptableBuffDebuff.nameID);

        //increase the variable that will store the temp attack
        buffDebuffClass.tempVariable += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);

        //increase character attack
        EntityClass entityClass = realTarget.GetComponent<EntityClass>();
        entityClass.attack += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);





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
