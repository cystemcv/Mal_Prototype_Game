using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_PoisonTarget", menuName = "CardAbility/Ability_PoisonTarget")]
public class Ability_PoisonTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {
        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to an enemy";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {


        base.OnPlayCard(cardScript, character, CombatManager.Instance.targetClicked);



        if (base.typeOfAttack == SystemManager.TypeOfAttack.MELLEE
            || base.typeOfAttack == SystemManager.TypeOfAttack.PROJECTILE)
        {
            InvokeHelper.Instance.Invoke(() => OnCompleteBase(cardScript, character), base.timeToGetToTarget);

        }
        else
        {
            ProceedToAbility(cardScript, character);
        }



    }

    private void OnCompleteBase(CardScript cardScript, GameObject character)
    {
        // Proceed with animation and sound after the movement
        ProceedToAbility(cardScript, character);
    }

    private void ProceedToAbility(CardScript cardScript, GameObject character)
    {

        BuffSystemManager.Instance.AddDebuffToEnemyTarget(this, CombatManager.Instance.targetClicked, 3);

    }

    //debuff

    public override bool OnEnemyTurnStart( GameObject target)
    {
        CombatManager.Instance.AdjustHealth(target, 1, false, SystemManager.AdjustNumberModes.ATTACK);

        //activated
        return true;
    }


}
