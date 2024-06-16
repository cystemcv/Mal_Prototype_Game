using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamagePierceSingleTarget", menuName = "CardAbility/Ability_DamagePierceSingleTarget")]
public class Ability_DamagePierceSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Deal " + GetAbilityVariable(cardScript) + " to an enemy (Ignore Block)";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {


        base.OnPlayCard(cardScript, character, CombatManager.Instance.targetClicked);



        if (base.runToTarget)
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

        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, GetAbilityVariable(cardScript), true, SystemManager.AdjustNumberModes.ATTACK);

    }


}
