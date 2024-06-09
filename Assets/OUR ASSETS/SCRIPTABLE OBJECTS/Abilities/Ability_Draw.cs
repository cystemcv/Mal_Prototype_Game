using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_Draw", menuName = "CardAbility/Ability_Draw")]
public class Ability_Draw : ScriptableCardAbility
{

   // public int damage;



    public override string AbilityDescription(CardScript cardScript)
    {
        string keyword = base.AbilityDescription(cardScript);
        string description = "Draw " + GetAbilityVariable(cardScript) + " cards";
        string final = keyword + " : " + description;

        return final;
    }

    public override void OnPlayCard(CardScript cardScript)
    {
        base.OnPlayCard(cardScript);

        Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = GetAbilityVariable(cardScript);

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
