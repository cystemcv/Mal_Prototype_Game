using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_DamageRandomTarget", menuName = "CardAbility/Ability_DamageRandomTarget")]
public class Ability_DamageRandomTarget : ScriptableCardAbility
{

    // public int damage;
    private GameObject targetEnemy;


    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {
        int cardDmg = GetAbilityVariable(cardScript);
        int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, null);

        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Deal " + cardDmg + DeckManager.Instance.GetBonusAttackAsDescription(cardDmg, calculatedDmg) + " to a random enemy";
        string final = keyword + " : " + description;

        return final;
    }




    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {
        //get all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int randomNmbr = Random.Range(0, enemies.Length);

        targetEnemy = enemies[randomNmbr];

        base.OnPlayCard(cardScript, character, targetEnemy);


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
        int calculatedDmg = CombatManager.Instance.CalculateCharacterDmg(GetAbilityVariable(cardScript), character, targetEnemy);
        CombatManager.Instance.AdjustHealth(targetEnemy, GetAbilityVariable(cardScript), false, SystemManager.AdjustNumberModes.ATTACK);

    }


}
