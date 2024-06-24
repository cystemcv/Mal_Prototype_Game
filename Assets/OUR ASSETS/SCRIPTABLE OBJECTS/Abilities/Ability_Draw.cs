using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_Draw", menuName = "CardAbility/Ability_Draw")]
public class Ability_Draw : ScriptableCardAbility
{

    [Header("UNIQUE")]
    public int empty;


    public override string AbilityDescription(CardScript cardScript, GameObject entity)
    {
        string keyword = base.AbilityDescription(cardScript, entity);
        string description = "Draw " + GetAbilityVariable(cardScript) + " cards";
        string final = keyword + " : " + description;

        return final;
    }



    public override void OnPlayCard(CardScript cardScript, GameObject entity, GameObject target)
    {
        base.OnPlayCard(cardScript, entity, null);

       // Debug.Log("Draw " + GetAbilityVariable(cardScript) +  " cards");

        int cardsToDraw = GetAbilityVariable(cardScript);

        DeckManager.Instance.DrawMultipleCards(cardsToDraw);

    }




}
