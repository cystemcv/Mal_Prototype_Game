using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_AddPoison", menuName = "CardAbility/Ability_AddPoison")]
public class Ability_AddPoison : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(ScriptableCard scriptableCard)
    {
        string keyword = base.AbilityDescription(scriptableCard);
        string description = "Add " + GetAbilityVariable(scriptableCard) + " Poison";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(ScriptableCard scriptableCard)
    {
        //base.OnPlayCard();

        //add poison to the target


    }




}
