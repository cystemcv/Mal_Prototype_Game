using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamagePierceSingleTarget", menuName = "CardAbility/Ability_DamagePierceSingleTarget")]
public class Ability_DamagePierceSingleTarget : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {
        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, null);

        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to an enemy (Ignore Block)";
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
        int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, CombatManager.Instance.targetClicked);
        CombatManager.Instance.AdjustHealth(CombatManager.Instance.targetClicked, calculatedDmg, true, SystemManager.AdjustNumberModes.ATTACK);

    }


}
