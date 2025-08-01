using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_IncreaseDefenceInfinite", menuName = "CardAbility/Ability_IncreaseDefenceInfinite")]
public class Ability_IncreaseDefenceInfinite : ScriptableCardAbility
{

    [Header("UNIQUE")]
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {

        string description = "";

        description = DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " <color=yellow>Defence</color> ";
        description += (infiniteDuration) ? "" : " for " + DeckManager.Instance.GetIntValueFromList(1, cardAbilityClass.abilityIntValueList) + " turns";

        string final = description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target 

            realTarget = CombatCardHandler.Instance.targetClicked;

        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);



        //EntityClass entityClass = BuffSystemManager.Instance.AddBuffDebuff(entity, this, DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList),0);
        //entityClass.defence += DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList);
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

        target.GetComponent<EntityClass>().defence -= buffDebuffClass.tempValue;


    }




}
