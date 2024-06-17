using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_Draw", menuName = "CardAbility/Ability_Draw")]
public class Ability_Draw : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript, GameObject character)
    {
        string keyword = base.AbilityDescription(cardScript, character);
        string description = "Draw " + GetAbilityVariable(cardScript) + " cards";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject character, GameObject target)
    {
        base.OnPlayCard(cardScript,character, null);

       // Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = GetAbilityVariable(cardScript);

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
