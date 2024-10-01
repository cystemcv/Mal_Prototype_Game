using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScriptableCard;

[CreateAssetMenu(fileName = "Ability_PoisonTarget", menuName = "CardAbility/Ability_PoisonTarget")]
public class Ability_PoisonTarget : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;
    private GameObject realTarget;

    public override string AbilityDescription(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, cardAbilityClass, entity);
        string description = "Deal " + DeckManager.Instance.GetIntValueFromList(0, cardAbilityClass.abilityIntValueList) + " to an enemy";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, CardAbilityClass cardAbilityClass, GameObject entity, SystemManager.ControlBy controlBy)
    {
        //assign target 

            realTarget = CombatCardHandler.Instance.targetClicked;
     
        base.OnPlayCard(cardScript, cardAbilityClass, entity, controlBy);


            ProceedToAbility(cardScript, entity);




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
        Combat.Instance.AdjustTargetHealth(target, 1, false, SystemManager.AdjustNumberModes.ATTACK);

        //activated
        return true;
    }


}
